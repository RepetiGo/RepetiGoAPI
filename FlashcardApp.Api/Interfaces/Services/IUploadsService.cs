using FlashcardApp.Api.Dtos.UploadDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IUploadsService
    {
        Task<ImageUploadResponse> UploadImageAsync(IFormFile formFile);
        Task<ImageUploadResponse> UploadImageAsync(byte[] bytes);
        Task<ImageUploadResponse> DeleteImageAsync(string oldAvatarPublicId);
    }
}
