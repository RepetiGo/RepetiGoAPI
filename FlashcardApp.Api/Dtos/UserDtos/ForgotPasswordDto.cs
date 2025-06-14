namespace FlashcardApp.Api.Dtos.UserDtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; }
    }
}
