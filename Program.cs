using Microsoft.Extensions.Configuration;
using ReadingEmailsGraphAPI.Repositories;
using ReadingEmailsGraphAPI.Services;
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

        // Create an instance of GraphApiService
        GraphApiService graphApiService = new GraphApiService(config);

        // Specify the email address to fetch unread messages
        string userEmail = config["GraphMail:Email"];

        try
        {
            // Call the GetUnreadMessagesAsync method to fetch unread messages
            var unreadMessages = await graphApiService.GetUnreadMessagesAsync(userEmail);

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
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
        }       
    } 
}
