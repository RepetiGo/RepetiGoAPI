﻿namespace RepetiGo.Api.Dtos.CardDtos
{
    public class UpdateCardRequest
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

        public IFormFile? ImageFile { get; set; }
    }
}
