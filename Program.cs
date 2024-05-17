using ReadingEmailsGraphAPI.Services;

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