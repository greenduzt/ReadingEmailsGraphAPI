using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using ReadingEmailsGraphAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            Console.WriteLine($"Name: {email.Name}");
            Console.WriteLine($"From {email.From}");
            Console.WriteLine($"Arrived DateTime: {email.ReceivedDateTime}");
            Console.WriteLine($"Subject: {email.Subject}");            
            Console.WriteLine($"Message: {email.Message}");

            //Process attachments only if the Attachments collection is not null
            if (message.Attachments != null)
            {
                email.Status = "Attachment available - ";
                // Process attachments
                foreach (var attachment in message.Attachments)
                {
                    email.Status += $"{attachment.ContentType} | ";

                    if (attachment is FileAttachment fileAttachment && fileAttachment.ContentType == "application/pdf")
                    {
                        // Download the attachment
                        var attachmentStream = fileAttachment.ContentBytes;
                        var attachmentFileName = fileAttachment.Name;

                        // Specify the folder path to save the attachment
                        var attachmentFolderPath = @"d:\Attachments\";

                        // naSave the attachment to the specified folder
                        var attachmentFilePath = Path.Combine(attachmentFolderPath, attachmentFileName);
                        System.IO.File.WriteAllBytes(attachmentFilePath, attachmentStream);

                        Console.WriteLine($"PDF attachment {attachmentFileName} downloaded to {attachmentFilePath} for email {message.Id}");

                        email.FileName = fileAttachment.Name;
                    }                    
                }
            }
            else
            {
                Console.WriteLine("No attachments found for the email.");
                email.Status += "Attachment not found";
            }


            return email;
        }
    }
}
