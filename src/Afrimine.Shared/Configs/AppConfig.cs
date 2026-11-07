namespace Afrimine.Shared.Configs
{
    public class AppConfig
    {
        public string JwtKey { get; set; } = default!;
        public string JwtIssuer { get; set; } = default!;
        public string JwtAudience { get; set; } = default!;
        public int JwtExpirationMinutes { get; set; } = default!;
        public string ResendApiKey { get; set; } = default!;
        public string ResendSender { get; set; } = default!;
        public string ResendEndpoint { get; set; } = default!;
        public string PlatformName { get; set; } = default!;
        public string PayscrowApiKey { get; set; } = string.Empty;
        public string PayscrowBaseUrl { get; set; } = "https://api.payscrow.dev";
        public string PayscrowReturnUrl { get; set; } = string.Empty;
        public string PayscrowWebhookUrl { get; set; } = string.Empty;
        public string GoogleClientId { get; set; } = string.Empty;
    }

    public class CloudinaryConfig
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }
}