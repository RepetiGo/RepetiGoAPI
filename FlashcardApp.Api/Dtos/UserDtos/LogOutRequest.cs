namespace FlashcardApp.Api.Dtos.UserDtos
{
    public class LogOutRequest
    {
        [Display(Name = "Refresh Token")]
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}