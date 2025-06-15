using FlashcardApp.Api.Dtos.UploadDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IUploadsService
    {
        Task<ImageUploadResponseDto> UploadImageAsync(IFormFile formFile);
        Task<ImageUploadResponseDto> DeleteImageAsync(string oldAvatarPublicId);
    }
}
