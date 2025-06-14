namespace FlashcardApp.Api.Dtos.UserDtos
{
    public class ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; } = string.Empty;
    }
}
