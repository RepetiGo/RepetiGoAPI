namespace RepetiGo.Api.Dtos.UserDtos
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "The email address is not valid.")]
        public string Email { get; set; } = string.Empty;
    }
}
