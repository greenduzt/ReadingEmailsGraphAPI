using CoreLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Serilog;
using System.Text;
using System.Text.RegularExpressions;

namespace ReadingEmailsGraphAPI.Services
{
    /* Responsible for processing emails */
    public class EmailProcessingService
    {
        private readonly IConfiguration _configuration;

        public EmailProcessingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConvertHtmlToPlainText(string html)
        {
            // Remove HTML tags using regular expression
            string plainText = Regex.Replace(html, "<[^>]*>", "");
            return plainText;
        }

        public Email ProcessMessage(Microsoft.Graph.Message message)
        {
            Email email = new Email
            {
                Name = message.From.EmailAddress.Name,
                From = message.From.EmailAddress.Address,
                ReceivedDateTime = message.ReceivedDateTime?.DateTime ?? DateTime.MinValue,
                Subject = message.Subject,
                Message = ConvertHtmlToPlainText(message.Body.Content),
               
            };
            Log.Information($"Name: {email.Name} | From {email.From} | Arrived DateTime: {email.ReceivedDateTime} | Subject: {email.Subject} | Message: {email.Message}");
         
            //Process attachments only if the Attachments collection is not null
            if (message.Attachments != null)
            {
                email.Status = "Attachment available - ";
                Log.Information("Attachment available");
                StringBuilder stringBuilder = new StringBuilder();

                // Process attachments
                foreach (var attachment in message.Attachments)
                {
                    email.Status += $"{attachment.ContentType} | ";
                    stringBuilder.Append($"{attachment.ContentType} | ");

                    if (attachment is FileAttachment fileAttachment && fileAttachment.ContentType == "application/pdf")
                    {
                        // Download the attachment
                        var attachmentStream = fileAttachment.ContentBytes;
                        var attachmentFileName = fileAttachment.Name;

                        // Specify the folder path to save the attachment
                        var attachmentFolderPath = _configuration["Attachment:DownloadPath"];

                        // aSave the attachment to the specified folder
                        var attachmentFilePath = Path.Combine(attachmentFolderPath, attachmentFileName);
                        try
                        {
                            System.IO.File.WriteAllBytes(attachmentFilePath, attachmentStream);

                            stringBuilder.Append($"PDF attachment {attachmentFileName} downloaded to {attachmentFilePath} for email {message.Id}");

                            email.FileName = fileAttachment.Name;
                        }
                        catch(Exception ex)
                        {
                            Log.Error(ex, $"Error occurred while writing file: {attachmentFilePath}");
                            throw;
                        }
                    }
                }

                Log.Information(stringBuilder.ToString());
            }
            else
            {
                Log.Information("No attachments found for the email.");
                email.Status += "Attachment not found";
            }


            return email;
        }
    }
}
