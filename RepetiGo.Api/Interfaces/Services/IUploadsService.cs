﻿using RepetiGo.Api.Dtos.UploadDtos;

namespace RepetiGo.Api.Interfaces.Services
{
    public interface IUploadsService
    {
        Task<ImageUploadResponse> UploadImageAsync(IFormFile formFile);
        Task<ImageUploadResponse> UploadImageAsync(byte[] bytes);
        Task<ImageUploadResponse> DeleteImageAsync(string oldAvatarPublicId);
        Task<ImageUploadResponse> CopyImageAsync(string imageUrlSource);
    }
}
