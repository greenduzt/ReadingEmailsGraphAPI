using Microsoft.Extensions.Configuration;

namespace ReadingEmailsGraphAPI.Repositories
{
    /** This factory creates an instance of EmailRepository object. It encapsulates the logic of creating these repository instances. **/
    public class EmailRepositoryFactory : IEmailRepositoryFactory
    {

        public IEmailRepository CreateEmailRepository(IConfiguration config)
        {       
            return new EmailRepository(config);
        }
    }
}
