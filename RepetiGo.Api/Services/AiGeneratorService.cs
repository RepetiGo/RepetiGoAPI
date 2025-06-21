
using System.Text.Json;

using Microsoft.Extensions.Options;

using Mscc.GenerativeAI;

using Polly.Registry;

using RepetiGo.Api.Dtos.GeneratedCardDtos;

namespace RepetiGo.Api.Services
{
    public class AiGeneratorService : IAiGeneratorService
    {
        private readonly GoogleGeminiConfig _googleGeminiConfig;
        private readonly IUploadsService _uploadsService;
        private readonly ResiliencePipelineProvider<string> _resiliencePipelineProvider;
        private readonly ILogger<AiGeneratorService> _logger;

        public AiGeneratorService(IOptions<GoogleGeminiConfig> options, IUploadsService uploadsService, ResiliencePipelineProvider<string> resiliencePipelineProvider, ILogger<AiGeneratorService> logger)
        {
            _googleGeminiConfig = options.Value;
            _uploadsService = uploadsService;
            _resiliencePipelineProvider = resiliencePipelineProvider;
            _logger = logger;
        }

        public async Task<GeneratedContentResult> GenerateCardContentAsync(GenerateRequest generateRequest)
        {
            if (string.IsNullOrWhiteSpace(generateRequest.Topic))
            {
                return new GeneratedContentResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Topic cannot be empty."
                };
            }

            try
            {
                var model = new VertexAI(projectId: _googleGeminiConfig.ProjectId).GenerativeModel(model: Model.Gemini25Flash);

                var pipeline = _resiliencePipelineProvider.GetPipeline("default");

                var responseJson = await pipeline.ExecuteAsync(async cancellationToken =>
                    await model.GenerateContent(
                        ResponseTemplate.GetPromptTemplate(generateRequest.Topic,
                        generateRequest.FrontText ?? string.Empty, generateRequest.BackText ?? string.Empty),
                        cancellationToken: cancellationToken)
                    );

                var response = JsonSerializer.Deserialize<Dtos.GeneratedCardDtos.JsonResult>(responseJson.Text!);
                if (response is not null && !string.IsNullOrWhiteSpace(response.FrontText) && !string.IsNullOrWhiteSpace(response.BackText))
                {
                    return new GeneratedContentResult
                    {
                        IsSuccess = true,
                        FrontText = response.FrontText.Trim(),
                        BackText = response.BackText.Trim()
                    };
                }

                _logger.LogWarning("AI service returned invalid or empty content. Response: {response}", responseJson.Text);
                return new GeneratedContentResult { IsSuccess = false, ErrorMessage = "The AI service did not return valid content." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating card content.");
                return new GeneratedContentResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"An error occurred while communicating with the AI service: {ex.Message}"
                };
            }
        }

        public async Task<GeneratedImageResult> GenerateCardImageAsync(GenerateRequest generateRequest)
        {
            if (string.IsNullOrWhiteSpace(generateRequest.Topic))
            {
                return new GeneratedImageResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Topic cannot be empty."
                };
            }

            try
            {
                var model = new VertexAI(projectId: _googleGeminiConfig.ProjectId).ImageGenerationModel(model: Model.Imagen4UltraExperimental);

                var pipeline = _resiliencePipelineProvider.GetPipeline("default");

                var imageResponse = await pipeline.ExecuteAsync(async cancellationToken =>
                    await model.GenerateImages(
                        ResponseTemplate.GetVisualIdeaPrompt(generateRequest.FrontText ?? string.Empty, generateRequest.BackText ?? string.Empty),
                        language: generateRequest.ImagePromptLanguage,
                        numberOfImages: 1,
                        enhancePrompt: generateRequest.EnhancePrompt,
                        aspectRatio: generateRequest.AspectRatio,
                        cancellationToken: cancellationToken
                    )
                );

                if (imageResponse is null || imageResponse.Predictions is null || imageResponse.Predictions.Count == 0)
                {
                    return new GeneratedImageResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "No images were generated."
                    };
                }

                foreach (var image in imageResponse.Predictions)
                {
                    // check if the image has bytes or base64 string
                    var imageBytes = image.ImageBytes;
                    if (imageBytes is null || imageBytes.Length == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(image.BytesBase64Encoded))
                        {
                            imageBytes = Convert.FromBase64String(image.BytesBase64Encoded);
                        }
                        else
                        {
                            return new GeneratedImageResult
                            {
                                IsSuccess = false,
                                ErrorMessage = "Generated image is empty."
                            };
                        }
                    }

                    // Upload the image to the uploads service
                    var uploadResult = await _uploadsService.UploadImageAsync(imageBytes);
                    if (!uploadResult.IsSuccess)
                    {
                        return new GeneratedImageResult
                        {
                            IsSuccess = false,
                            ErrorMessage = uploadResult.ErrorMessage
                        };
                    }

                    return new GeneratedImageResult
                    {
                        IsSuccess = true,
                        ImageUrl = uploadResult.SecureUrl,
                        ImagePublicId = uploadResult.PublicId
                    };
                }

                return new GeneratedImageResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Failed to upload the generated image."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating card image.");
                return new GeneratedImageResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"An error occurred while communicating with the AI service: {ex.Message}"
                };
            }
        }
    }
}
