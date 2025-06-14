using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using FlashcardApp.Api.Dtos.UploadDtos;

using Microsoft.Extensions.Options;

namespace FlashcardApp.Api.Services
{
    public class UploadsService : IUploadsService
    {
        private readonly CloudinaryConfig _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;

        public UploadsService(IOptions<CloudinaryConfig> options)
        {
            _cloudinaryConfig = options.Value;
            var account = new Account(
                _cloudinaryConfig.CloudName,
                _cloudinaryConfig.ApiKey,
                _cloudinaryConfig.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResponseDto> DeleteImageAsync(string oldAvatarPublicId)
        {
            if (string.IsNullOrEmpty(oldAvatarPublicId))
            {
                return new ImageUploadResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Public ID is required for deletion."
                };
            }
            var deleteParams = new DeletionParams(oldAvatarPublicId);
            var deletionResult = await _cloudinary.DestroyAsync(deleteParams);
            return new ImageUploadResponseDto
            {
                IsSuccess = deletionResult.Result == "ok",
                ErrorMessage = deletionResult.Error?.Message ?? string.Empty
            };
        }

        public async Task<ImageUploadResponseDto> UploadImageAsync(UploadRequestDto uploadRequestDto)
        {
            var file = uploadRequestDto.File;
            if (file == null || file.Length == 0)
            {
                return new ImageUploadResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = "Upload failed."
                };
            }

            var uploadResult = new ImageUploadResult();

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    UploadPreset = _cloudinaryConfig.UploadPreset,
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
                return new ImageUploadResponseDto
                {
                    IsSuccess = false,
                    ErrorMessage = uploadResult.Error.Message
                };
            }

            return new ImageUploadResponseDto
            {
                IsSuccess = true,
                SecureUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId
            };
        }
    }
}
