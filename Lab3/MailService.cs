using System.Net;
using System.Net.Mail;

namespace Lab3
{
    public interface IMailService
    {
        bool SendEmail(string toEmail, string subject, string body);
    }

    public class MailService(ILogger<MailService> logger, IConfiguration config) : IMailService
    {
        private readonly string _senderName = "Greenswamp Community";
        private readonly string _senderEmail = "admin@newdaysteam.ru";

        private readonly ILogger<MailService> _logger = logger;
        private readonly IConfiguration _config = config;

        public bool SendEmail(string toEmail, string subject, string body)
        {
            string _smtpServer = _config["Smtp:Server"];
            int _smtpPort = int.Parse(_config["Smtp:Port"]);
            string _smtpUsername = _config["Smtp:Username"];
            string _smtpPassword = _config["Smtp:Password"];
            
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {

                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    var fromAddress = new MailAddress(_senderEmail, _senderName);
                    var toAddress = new MailAddress(toEmail);

                    using var message = new MailMessage(fromAddress, toAddress);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    client.Send(message);
                }

                _logger.LogInformation("Email successfully sent to {ToEmail}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while sending email to {ToEmail}", toEmail);
                return false;
            }
        }
    }
}
