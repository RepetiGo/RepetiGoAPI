
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

        public AiGeneratorService(IOptions<GoogleGeminiConfig> options, IUploadsService uploadsService, ResiliencePipelineProvider<string> resiliencePipelineProvider)
        {
            _googleGeminiConfig = options.Value;
            _uploadsService = uploadsService;
            _resiliencePipelineProvider = resiliencePipelineProvider;
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

            var model = new GoogleAI(apiKey: _googleGeminiConfig.ApiKey).GenerativeModel(model: Model.Gemini25Flash);

            var responseJson = await model.GenerateContent(
                ResponseTemplate.GetPromptTemplate(generateRequest.Topic, generateRequest.FrontText ?? string.Empty, generateRequest.BackText ?? string.Empty)
            );

            try
            {
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
            }
            catch
            {
                return new GeneratedContentResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Failed to parse the response from the AI service."
                };
            }

            return new GeneratedContentResult
            {
                IsSuccess = false,
                ErrorMessage = "The AI service did not return valid content."
            };
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

            var model = new VertexAI(projectId: _googleGeminiConfig.ProjectId, region: _googleGeminiConfig.Region).ImageGenerationModel(model: Model.Imagen4UltraExperimental);
            //model.AccessToken = _googleGeminiConfig.AccessToken;

            var pipeline = _resiliencePipelineProvider.GetPipeline("default");

            ImageGenerationResponse imageResponse = await pipeline.ExecuteAsync(async cancellationToken =>
                imageResponse = await model.GenerateImages(
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
    }
}
