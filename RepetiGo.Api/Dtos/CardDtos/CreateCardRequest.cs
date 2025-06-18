namespace RepetiGo.Api.Dtos.CardDtos
{
    public class CreateCardRequest
    {
        [MinLength(1)]
        [MaxLength(500)]
        [Display(Name = "Front Text")]
        [Required(ErrorMessage = "Front text is required")]
        public string FrontText { get; set; } = string.Empty;

        [MinLength(1)]
        [MaxLength(500)]
        [Display(Name = "Back Text")]
        [Required(ErrorMessage = "Back text is required")]
        public string BackText { get; set; } = string.Empty;

        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Image Public ID")]
        public string? ImagePublicId { get; set; }
    }
}
