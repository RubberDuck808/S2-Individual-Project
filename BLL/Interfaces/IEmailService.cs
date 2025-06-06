using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent);
    }
}
