namespace Afrimine.Shared
{
    public class UserContext
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? IpAddress { get; set; }
    }
}
