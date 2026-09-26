using Afrimine.Shared.Contract;
using Hangfire;

namespace Afrimine.Shared.ExternalServices
{
    public class Notifications
    {
        public static void SendEmail(string email, string subject, string text, string html)
        {
            BackgroundJob.Enqueue<INotificationService>(a => a.SendEmailAsync(email, subject, text, html));
        }
    }
}
