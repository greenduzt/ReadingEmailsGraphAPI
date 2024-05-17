using CoreLibrary.Models;

namespace ReadingEmailsGraphAPI.Repositories
{
    public interface IEmailRepository
    {
        void AddEmail(Email email);
    }
}
