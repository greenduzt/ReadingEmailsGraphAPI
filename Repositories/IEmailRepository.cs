using ReadingEmailsGraphAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEmailsGraphAPI.Repositories
{
    public interface IEmailRepository
    {
        void AddEmail(Email email);
    }
}
