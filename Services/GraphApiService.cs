using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;

namespace ReadingEmailsGraphAPI.Services
{
    /* Responsible for fetching emails from the Graph API */
    public class GraphApiService
    {
        private readonly IConfiguration _configuration;
        private readonly GraphServiceClient _graphServiceClient;

        public GraphApiService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Initialize GraphServiceClient with authentication
            var credentials = new ClientSecretCredential(
                _configuration["GraphMail:TenantId"],
                _configuration["GraphMail:ClientId"],
                _configuration["GraphMail:ClientSecret"],
                new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });

            _graphServiceClient = new GraphServiceClient(credentials);
        }

        public async Task<List<Message>> GetUnreadMessagesAsync(string userEmail)
        {
            try
            {
                // Filter to retrieve only unread messages
                var queryOptions = new List<QueryOption>
                {
                    new QueryOption("$filter", "isRead eq false")
                };

                // Get the unread emails for a specific user.
                var messages = await _graphServiceClient.Users[userEmail].Messages
                    .Request(queryOptions)
                    .Expand("attachments")
                    .GetAsync();

                return messages.CurrentPage.ToList();
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"Error occurred while fetching unread messages: {ex.Message}");
                throw;
            }
        }

        public void MakeEmailRead(Message message)
        {
            //Mark the email as read
            var updatedMessage = new Message
            {
                IsRead = true
            };

            // Update the message to mark it as read
            _graphServiceClient.Users[_configuration["GraphMail:Email"]].Messages[message.Id]
                .Request()
                .UpdateAsync(updatedMessage)
                .Wait();
        }
    }
}
