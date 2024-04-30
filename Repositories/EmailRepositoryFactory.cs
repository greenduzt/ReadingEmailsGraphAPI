using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
