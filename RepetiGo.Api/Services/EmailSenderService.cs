
using System.Net.Mail;

using Microsoft.Extensions.Options;

namespace RepetiGo.Api.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailSettingsConfig _emailSettingsConfig;

        public EmailSenderService(IOptions<EmailSettingsConfig> options)
        {
            _emailSettingsConfig = options.Value;
        }

        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            var mailServer = _emailSettingsConfig.MailServer;
            var fromEmail = _emailSettingsConfig.FromMail;
            var password = _emailSettingsConfig.Password;
            var senderName = _emailSettingsConfig.SenderName;
            var port = _emailSettingsConfig.MailPort;

            // Create a new instance of SmtpClient
            var client = new SmtpClient(mailServer, port)
            {
                // Set the credentials for the SMTP client
                Credentials = new NetworkCredential(fromEmail, password),
                // Enable SSL for secure email sending
                EnableSsl = true
            };

            // Create a new MailAddress for the sender
            var fromAddress = new MailAddress(fromEmail, senderName);

            // Create a new MailMessage
            var mailMessage = new MailMessage
            {
                From = fromAddress, // Set the sender address
                Subject = subject, // Set the subject of the email
                Body = body, // Set the body of the email
                IsBodyHtml = isBodyHtml // Specify if the body is HTML or plain text
            };

            // Add the recipient email address to the MailMessage
            mailMessage.To.Add(toEmail);

            // Send the email asynchronously
            return client.SendMailAsync(mailMessage);
        }
    }
}
