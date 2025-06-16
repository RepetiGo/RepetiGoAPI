namespace RepetiGo.Api.ConfigModels
{
    public class OAuthConfig
    {
        public const string SectionName = "OAuth";

        [Required]
        public required GoogleConfig Google { get; set; }
    }
}
