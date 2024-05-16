using Microsoft.Extensions.Configuration;
using ReadingEmailsGraphAPI.Repositories;
using Serilog.Events;
using Serilog;
using CoreLibrary;

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
    }
}
