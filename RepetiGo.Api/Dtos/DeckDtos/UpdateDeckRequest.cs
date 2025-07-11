﻿namespace RepetiGo.Api.Dtos.DeckDtos
{
    public class UpdateDeckRequest
    {
        [MinLength(1)]
        [MaxLength(100)]
        [Display(Name = "Deck Name")]
        [Required(ErrorMessage = "Deck name is required")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Visibility")]
        [Required(ErrorMessage = "Visibility is required")]
        [EnumDataType(typeof(CardVisibility), ErrorMessage = "Invalid visibility type")]
        [Range(0, 1, ErrorMessage = "Visibility must be between 0 (Private) and 1 (Public)")]
        public CardVisibility Visibility { get; set; } = CardVisibility.Public;
    }
}
