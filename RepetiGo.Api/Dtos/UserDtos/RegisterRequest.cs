namespace RepetiGo.Api.Dtos.UserDtos
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, ErrorMessage = "Username length must be between 3 and 20 characters", MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and numbers")]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [MinLength(8)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}