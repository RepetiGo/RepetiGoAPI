namespace backend.Dtos.AuthDtos
{
    public class RefreshTokenDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
