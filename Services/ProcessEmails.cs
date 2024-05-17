using Microsoft.Extensions.Configuration;
using ReadingEmailsGraphAPI.Repositories;
using Serilog.Events;
using Serilog;
using CoreLibrary;
using CoreLibrary.Models;
using IEmailRepository = ReadingEmailsGraphAPI.Repositories.IEmailRepository;
using Microsoft.Graph;
using Microsoft.Extensions.Hosting;
using CoreLibrary.Data;
using Microsoft.Extensions.DependencyInjection;
using CoreLibrary.Data.Repositories;

namespace ReadingEmailsGraphAPI.Services
{
    public class ProcessEmails
    {
        // Encapsulating the calling method
        public async Task<Email> ProcessEmailAsync()
        {
            //Set up the config to load the user secrets
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<Program>(true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File($"{config["Logging:Path"]} - {DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt",
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: LogEventLevel.Debug,
                        shared: true)
                    .CreateLogger();

            var services = ConfigureServices(config); // Configure services
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var systemParameterRepository = serviceProvider.GetRequiredService<ISystemParameterRepository>();
                var hubSpotProductRepository = serviceProvider.GetRequiredService<IHubSpotProductRepository>();

                var parameters = await systemParameterRepository.GetAllAsync();
                foreach (var param in parameters)
                {
                    Console.WriteLine($"Type: {param.Type}, AttachmentLocation: {param.AttchmentLocation}");
                }

                var products = await hubSpotProductRepository.GetAllAsync();
                foreach (var product in products)
                {
                    Console.WriteLine($"Name: {product.Name}, SKU: {product.SKU}, Price: {product.Price}");
                }

            }

            Log.Information("---ReadingEmails Started---");

            // Create an instance of GraphApiService
            GraphApiService graphApiService = new GraphApiService(config);

            // Email address to fetch unread messages
            string userEmail = config["GraphMail:Email"];

            try
            {
                // Calling the GetUnreadMessagesAsync method to fetch unread messages
                var unreadMessages = await graphApiService.GetUnreadMessagesAsync(userEmail);

                if (unreadMessages != null)
                {
                    // Creating an instance of EmailProcessingService
                    EmailProcessingService emailProcessingService = new EmailProcessingService(config);

                    // Creating the factory
                    IEmailRepositoryFactory repositoryFactory = new EmailRepositoryFactory();

                    // Use the factory to create the email repository
                    IEmailRepository emailRepository = repositoryFactory.CreateEmailRepository(config);

                    // Process message using EmailProcessingService
                    Email email = emailProcessingService.ProcessMessage(unreadMessages);

                    // Add email to repository
                    emailRepository.AddEmail(email);

                    // Mark the email read
                    graphApiService.MakeEmailRead(unreadMessages);

                    // Allocate the attachment path to email object
                    //var filePath = CoreLibrary.Functions.CoreFunctions.GetPOFilePath();
                    
                    //email.FilePath = filePath;
                    
                    //if(string.IsNullOrWhiteSpace(filePath))
                    //{
                    //    Log.Error("PO file path not found");
                    //    // Set default file path
                    //    //email.FilePath = "";
                    //}

                    return email;
                }
                else
                {
                    Log.Information("No new emails found");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unhandled exception occurred");
            }
            finally
            {
                Log.Information("---ReadingEmails Ended---");
                // Close and flush the Serilog logger
                Log.CloseAndFlush();
            }

            // Returns null if no unread email is found or any error 
            return null;
        }

        private static IServiceCollection ConfigureServices(IConfiguration config)
        {
            var services = new ServiceCollection();
            services.AddDataAccessServices(config); // Pass IConfiguration
            return services;
        }

    }
}
