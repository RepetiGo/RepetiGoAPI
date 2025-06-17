namespace RepetiGo.Api.Dtos.CardDtos
{
    public class PreviewCardResponse
    {
        public string FrontText { get; set; } = string.Empty;

        public string BackText { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string? ImagePublicId { get; set; }
    }
}
