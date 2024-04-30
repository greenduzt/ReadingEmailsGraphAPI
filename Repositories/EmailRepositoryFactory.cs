using Microsoft.Extensions.Configuration;

namespace ReadingEmailsGraphAPI.Repositories
{
    public class EmailRepositoryFactory : IEmailRepositoryFactory
    {

        public IEmailRepository CreateEmailRepository(IConfiguration config)
        {       
            return new EmailRepository(config);
        }
    }
}
