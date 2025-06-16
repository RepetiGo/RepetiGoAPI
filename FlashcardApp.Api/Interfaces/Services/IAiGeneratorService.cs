using FlashcardApp.Api.Dtos.GeneratedCardDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IAiGeneratorService
    {
        Task<GeneratedContentResult> GenerateCardContentAsync(GenerateRequest generateRequest);
        Task<GeneratedImageResult> GenerateCardImageAsync(GenerateRequest generateRequest);
    }
}
