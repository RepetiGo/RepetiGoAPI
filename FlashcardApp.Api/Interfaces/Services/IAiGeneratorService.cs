using FlashcardApp.Api.Dtos.GeneratedContentDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IAiGeneratorService
    {
        Task<GeneratedContentResult> GenerateCardContentAsync(GenerateRequest generateRequest);
        Task<GeneratedImageResult> GenerateCardImageAsync(GenerateRequest generateRequest);
    }
}
