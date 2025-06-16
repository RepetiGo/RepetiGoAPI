namespace RepetiGo.Api.Dtos.UploadDtos
{
    public class ImageUploadResponse
    {
        public bool IsSuccess { get; set; }
        public string SecureUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
