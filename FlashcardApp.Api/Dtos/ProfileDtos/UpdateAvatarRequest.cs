namespace RepetiGo.Api.Dtos.ProfileDtos
{
    public class UpdateAvatarRequest
    {
        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = null!;
    }
}
