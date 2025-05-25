namespace backend.Dtos.AuthDtos
{
    public class LogoutDTo
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
