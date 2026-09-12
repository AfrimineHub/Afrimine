using Afrimine.Shared.Configs;
using Afrimine.Shared.Contract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrimine.Shared.ExternalServices
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient httpClient;
        private readonly AppConfig settings;
        private readonly ILogger<NotificationService> logger;
        private readonly JsonSerializerOptions camelCaseOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public NotificationService(HttpClient httpClient,
            IOptions<AppConfig> options,
            ILogger<NotificationService> logger)
        {
            this.httpClient = httpClient;
            this.settings = options.Value;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string text, string html)
        {
            logger.LogInformation($"[SendEmailAsync] Sending email to {to}");
            var json = JsonSerializer.Serialize(new EmailRequest
            {
                From = $"{settings.PlatformName} <{settings.ResendSender}>",
                To = to,
                Subject = subject,
                Text = text,
                Html = html
            }, camelCaseOption);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{settings.ResendEndpoint}/email");
            request.Headers.Add("Authorization", settings.ResendApiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            var rawResponse = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) 
            {
                logger.LogError($"[SendEmailAsync] Http failure {response.StatusCode}: {rawResponse}");
            }
            else
            {
                logger.LogInformation($"[SendEmailAsync] Sent email to {to}");
            }
        }
    }
}
