using Microsoft.Extensions.Configuration;
using ReadingEmailsGraphAPI.Repositories;
using ReadingEmailsGraphAPI.Services;
using Serilog.Events;
using Serilog;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main(string[] args)
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

        // Specify the email address to fetch unread messages
        string userEmail = config["GraphMail:Email"];

        try
        {
            // Call the GetUnreadMessagesAsync method to fetch unread messages
            var unreadMessages = await graphApiService.GetUnreadMessagesAsync(userEmail);

            if (unreadMessages.Count > 0)
            {
                // Create an instance of EmailProcessingService
                EmailProcessingService emailProcessingService = new EmailProcessingService(config);

                // Create the factory
                IEmailRepositoryFactory repositoryFactory = new EmailRepositoryFactory();

                // Use the factory to create the email repository
                IEmailRepository emailRepository = repositoryFactory.CreateEmailRepository(config);

                // Process the unread messages and add them to the repository
                foreach (var message in unreadMessages)
                {
                    // Process message using EmailProcessingService
                    var email = emailProcessingService.ProcessMessage(message);

                    // Add email to repository
                    emailRepository.AddEmail(email);

                    // Mark the email read
                    graphApiService.MakeEmailRead(message);
                }
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
        }
    } 
}
