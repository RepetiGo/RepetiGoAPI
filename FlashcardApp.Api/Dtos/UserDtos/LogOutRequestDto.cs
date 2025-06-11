namespace FlashcardApp.Api.Dtos.UserDtos
{
    public class LogOutRequestDto
    {
        [Display(Name = "Refresh Token")]
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}