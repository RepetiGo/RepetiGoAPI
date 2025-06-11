namespace backend.Dtos.UserDtos
{
    public class LogOutRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}