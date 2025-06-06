using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            string name = _config["MailboxAddress:Name"];
            string address = _config["MailboxAddress:Address"];

            var message = new MimeMessage();
            message.Sender = new MailboxAddress(name, address);
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = htmlContent };
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.From.Add(MailboxAddress.Parse(address));

            using var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            try
            {
                await smtp.ConnectAsync("mailrelay.fhict.local", 25, SecureSocketOptions.None);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
