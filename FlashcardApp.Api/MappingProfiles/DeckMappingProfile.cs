using AutoMapper;

using FlashcardApp.Api.Dtos.DeckDtos;

namespace FlashcardApp.Api.MappingProfiles
{
    public class DeckMappingProfile : Profile
    {
        public DeckMappingProfile()
        {
            CreateMap<CreateDeckRequestDto, Deck>();
            CreateMap<UpdateDeckRequestDto, Deck>();
            CreateMap<Deck, DeckResponseDto>();
        }
    }
}
