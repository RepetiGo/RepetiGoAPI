using AutoMapper;

using FlashcardApp.Api.Dtos.DeckDtos;

namespace FlashcardApp.Api.MappingProfiles
{
    public class DeckMappingProfile : Profile
    {
        public DeckMappingProfile()
        {
            CreateMap<CreateDeckRequest, Deck>();
            CreateMap<UpdateDeckRequest, Deck>();
            CreateMap<Deck, DeckResponse>();
        }
    }
}
