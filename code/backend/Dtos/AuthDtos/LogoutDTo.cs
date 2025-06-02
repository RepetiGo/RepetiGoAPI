namespace backend.Dtos.AuthDtos
{
    public class LogoutDTo
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}