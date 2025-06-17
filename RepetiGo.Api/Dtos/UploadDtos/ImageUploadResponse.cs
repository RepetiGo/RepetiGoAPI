namespace RepetiGo.Api.Dtos.UploadDtos
{
    public class ImageUploadResponse
    {
        public bool IsSuccess { get; set; }
        public string? SecureUrl { get; set; }
        public string? PublicId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
