using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEmailsGraphAPI.Repositories
{
    /* Create an interface that declares a method for creating instances of the IEmailRepository interface */
    public interface IEmailRepositoryFactory
    {
        IEmailRepository CreateEmailRepository(IConfiguration config);
    }
}
