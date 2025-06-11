namespace backend.Dtos.UserDtos
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}