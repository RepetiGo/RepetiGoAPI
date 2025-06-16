namespace RepetiGo.Api.ConfigModels
{
    public class EmailSettingsConfig
    {
        public const string SectionName = "EmailSettings";

        [Required]
        public required string MailServer { get; set; }

        [Required]
        public required int MailPort { get; set; }

        [Required]
        public required string SenderName { get; set; }

        [Required]
        public required string FromMail { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
