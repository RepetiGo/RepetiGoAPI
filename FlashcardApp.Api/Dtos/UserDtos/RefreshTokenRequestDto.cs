namespace FlashcardApp.Api.Dtos.UserDtos
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}