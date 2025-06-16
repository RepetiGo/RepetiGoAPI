using AutoMapper;

using FlashcardApp.Api.Dtos.CardDtos;

namespace FlashcardApp.Api.MappingProfiles
{
    public class CardMappingProfile : Profile
    {
        public CardMappingProfile()
        {
            CreateMap<CreateCardRequest, Card>();
            CreateMap<UpdateCardRequest, Card>();
            CreateMap<Card, CardResponse>();
        }
    }
}
