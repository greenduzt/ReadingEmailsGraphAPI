using CoreLibrary;
using Microsoft.Extensions.Configuration;
using ReadingEmailsGraphAPI.DB;

namespace ReadingEmailsGraphAPI.Repositories
{
    public class EmailRepository:IEmailRepository
    {
        private readonly IConfiguration _config;

        public EmailRepository(IConfiguration config)
        {
            _config = config;
            DBConfiguration.Config = config;
            DBConfiguration.Initialize();
        }
        public void AddEmail(Email email)
        {
            // Call the DBAccess class to add the email to the database
            DBAccess.AddToEmailReceived(email);
        }
    }
}
