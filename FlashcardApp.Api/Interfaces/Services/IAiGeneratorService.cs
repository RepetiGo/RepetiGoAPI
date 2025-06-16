using RepetiGo.Api.Dtos.GeneratedCardDtos;

namespace RepetiGo.Api.Interfaces.Services
{
    public interface IAiGeneratorService
    {
        Task<GeneratedContentResult> GenerateCardContentAsync(GenerateRequest generateRequest);
        Task<GeneratedImageResult> GenerateCardImageAsync(GenerateRequest generateRequest);
    }
}
