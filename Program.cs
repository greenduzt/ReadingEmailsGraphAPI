using Microsoft.Extensions.Configuration;
using ReadingEmailsGraphAPI.Repositories;
using ReadingEmailsGraphAPI.Services;
using Serilog;
using Serilog.Events;

namespace ReadingEmailsGraphAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProcessEmails processEmails = new ProcessEmails();
            await processEmails.ProcessEmailAsync();
        }

        
    }

}