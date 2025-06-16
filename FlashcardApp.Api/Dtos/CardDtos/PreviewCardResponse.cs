namespace FlashcardApp.Api.Dtos.CardDtos
{
    public class PreviewCardResponse
    {
        public string FrontText { get; set; } = string.Empty;

        public string BackText { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string ImagePublicId { get; set; } = string.Empty;
    }
}
