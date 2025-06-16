namespace RepetiGo.Api.Dtos.ProfileDtos
{
    public class UpdateUsernameRequest
    {
        [Required(ErrorMessage = "New username is required")]
        [StringLength(20, ErrorMessage = "Username length must be between 3 and 20 characters", MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and numbers")]
        public string NewUsername { get; set; } = string.Empty;
    }
}
