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
    }
}
