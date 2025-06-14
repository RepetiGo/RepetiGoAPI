namespace FlashcardApp.Api.Dtos.UserDtos
{
    public class ResetPasswordDto
    {
            [Required(ErrorMessage = "Please enter your email address.")]
            [EmailAddress(ErrorMessage = "The email address is not valid.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password, ErrorMessage = "Invalid password format.")]
            [Display(Name = "New Password")]
            public string Password { get; set; }
            [Required(ErrorMessage = "Please confirm your password.")]
            [DataType(DataType.Password, ErrorMessage = "Invalid password format.")]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "Password and Confirm Password must match.")]
            public string ConfirmPassword { get; set; }
            [Required(ErrorMessage = "The password reset code is required.")]
            public string code { get; set; }

    }
}
