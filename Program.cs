using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    { 
        //Set up the config to load the user secrets
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Program>(true)
            .Build();

        // Define the credentials.        
        var credentials = new ClientSecretCredential(
            config["GraphMail:TenantId"],
            config["GraphMail:ClientId"],
            config["GraphMail:ClientSecret"],
            new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });

        // Define a new Microsoft Graph service client.            
        GraphServiceClient _graphServiceClient = new GraphServiceClient(credentials);

        // Filter to retrieve only unread messages
        var queryOptions = new List<QueryOption>
        {
            new QueryOption("$filter", "isRead eq false")
        };

        // Get the unread emails for a specific user.
        var messages = _graphServiceClient.Users[config["GraphMail:Email"]].Messages
            .Request(queryOptions)
            .Expand("attachments")
            .GetAsync()
            .Result;

        foreach (var message in messages)
        {
            Console.WriteLine($"Name: {message.From.EmailAddress.Name}");
            Console.WriteLine($"From { message.From.EmailAddress.Address}");
            Console.WriteLine($"Arrived At: {message.ReceivedDateTime?.ToString("yyyy-MM-dd HH:mm:ss")}");
            Console.WriteLine($"Subject: {message.Subject}");
            // Convert HTML to plain text
            string plainTextBody = ConvertHtmlToPlainText(message.Body.Content);
            Console.WriteLine($"Message: {plainTextBody}");

            // Process attachments only if the Attachments collection is not null
            if (message.Attachments != null)
            {
                // Process attachments
                foreach (var attachment in message.Attachments)
                {
                    if (attachment is FileAttachment fileAttachment && fileAttachment.ContentType == "application/pdf")
                    {
                        // Download the attachment
                        var attachmentStream = fileAttachment.ContentBytes;
                        var attachmentFileName = fileAttachment.Name;

                        // Specify the folder path to save the attachment
                        var attachmentFolderPath = @"d:\Attachments\";

                        // Save the attachment to the specified folder
                        var attachmentFilePath = Path.Combine(attachmentFolderPath, attachmentFileName);
                        System.IO.File.WriteAllBytes(attachmentFilePath, attachmentStream);

                        Console.WriteLine($"PDF attachment {attachmentFileName} downloaded to {attachmentFilePath} for email {message.Id}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No attachments found for the email.");
            }

            // Mark the email as read
            var updatedMessage = new Message
            {
                IsRead = true
            };

            // Update the message to mark it as read
            _graphServiceClient.Users[config["GraphMail:Email"]].Messages[message.Id]
                .Request()
                .UpdateAsync(updatedMessage)
                .Wait();

            Console.WriteLine("---");
        }

        static string ConvertHtmlToPlainText(string html)
        {
            // Remove HTML tags using regular expression
            string plainText = Regex.Replace(html, "<[^>]*>", "");
            return plainText;
        }
    }
}
