namespace backend.Dtos.AuthDtos
{
    public class RefreshTokenDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}