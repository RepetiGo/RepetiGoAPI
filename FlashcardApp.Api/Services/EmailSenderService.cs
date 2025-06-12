
using System.Net.Mail;

namespace FlashcardApp.Api.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            var mailServer = _configuration["EmailSettings:MailServer"] ?? throw new InvalidOperationException("Mail server not configured.");
            var fromEmail = _configuration["EmailSettings:FromEmail"] ?? throw new InvalidOperationException("From email not configured.");
            var password = _configuration["EmailSettings:Password"] ?? throw new InvalidOperationException("Email password not configured.");
            var senderName = _configuration["EmailSettings:SenderName"] ?? throw new InvalidOperationException("Sender name not configured.");
            var port = int.TryParse(_configuration["EmailSettings:Port"], out var parsedPort) ? parsedPort : 587;

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
