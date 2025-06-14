namespace FlashcardApp.Api.Dtos.UploadDtos
{
    public class UploadRequestDto
    {
        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } = null!;
    }
}
