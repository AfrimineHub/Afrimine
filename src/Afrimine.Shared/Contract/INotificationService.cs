namespace Afrimine.Shared.Contract
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string text, string html);
    }
}
