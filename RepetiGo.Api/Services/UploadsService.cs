using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

using Microsoft.Extensions.Options;

using RepetiGo.Api.Dtos.UploadDtos;

namespace RepetiGo.Api.Services
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

        public async Task<ImageUploadResponse> CopyImageAsync(string imageUrlSource)
        {
            if (string.IsNullOrEmpty(imageUrlSource))
            {
                return new ImageUploadResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Image URL is required for copying."
                };
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageUrlSource),
                UploadPreset = _cloudinaryConfig.UploadPreset,
            };

            var copyResult = await _cloudinary.UploadAsync(uploadParams);
            if (copyResult.Error is not null)
            {
                return new ImageUploadResponse
                {
                    IsSuccess = false,
                    ErrorMessage = copyResult.Error.Message
                };
            }

            return new ImageUploadResponse
            {
                IsSuccess = true,
                SecureUrl = copyResult.SecureUrl.ToString(),
                PublicId = copyResult.PublicId
            };
        }

        public async Task<ImageUploadResponse> DeleteImageAsync(string oldAvatarPublicId)
        {
            if (string.IsNullOrEmpty(oldAvatarPublicId))
            {
                return new ImageUploadResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Public ID is required for deletion."
                };
            }

            var deleteParams = new DeletionParams(oldAvatarPublicId);
            var deletionResult = await _cloudinary.DestroyAsync(deleteParams);
            return new ImageUploadResponse
            {
                IsSuccess = deletionResult.Result == "ok",
                ErrorMessage = deletionResult.Error?.Message ?? string.Empty
            };
        }

        public async Task<ImageUploadResponse> UploadImageAsync(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return new ImageUploadResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Upload failed."
                };
            }

            var uploadResult = new ImageUploadResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            using (var stream = new MemoryStream(bytes))
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription("image", stream),
                    UploadPreset = _cloudinaryConfig.UploadPreset,
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            watch.Stop();
            Console.WriteLine($"Image upload took {watch.ElapsedMilliseconds} ms");

            if (uploadResult.Error != null)
            {
                return new ImageUploadResponse
                {
                    IsSuccess = false,
                    ErrorMessage = uploadResult.Error.Message
                };
            }

            return new ImageUploadResponse
            {
                IsSuccess = true,
                SecureUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId
            };
        }

        public async Task<ImageUploadResponse> UploadImageAsync(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return new ImageUploadResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "Upload failed."
                };
            }

            var uploadResult = new ImageUploadResult();

            using (var stream = formFile.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    UploadPreset = _cloudinaryConfig.UploadPreset,
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
                return new ImageUploadResponse
                {
                    IsSuccess = false,
                    ErrorMessage = uploadResult.Error.Message
                };
            }

            return new ImageUploadResponse
            {
                IsSuccess = true,
                SecureUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId
            };
        }
    }
}
