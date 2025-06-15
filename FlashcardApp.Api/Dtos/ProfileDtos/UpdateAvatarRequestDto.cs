namespace FlashcardApp.Api.Dtos.ProfileDtos
{
    public class UpdateAvatarRequestDto
    {
        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = null!;
    }
}
