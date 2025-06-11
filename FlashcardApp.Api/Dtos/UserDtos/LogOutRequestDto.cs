namespace FlashcardApp.Api.Dtos.UserDtos
{
    public class LogOutRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}