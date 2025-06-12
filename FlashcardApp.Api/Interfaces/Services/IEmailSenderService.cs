namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IEmailSenderService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false);
    }
}
