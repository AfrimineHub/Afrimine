namespace Afrimine.Shared.Configs
{
    internal class EmailRequest
    {
        public string From { get; set; } = default!;
        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Html { get; set; } = default!;
        public string Text { get; set; } = default!;
    }

    internal class EmailAttachments
    {
        public string File { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string Content_type { get; set; } = default!;
    }

    internal class EmailRequestWithAttachments :EmailRequest
    {
        public List<EmailAttachments> Attachments { get; set; } = [];
    }
}
