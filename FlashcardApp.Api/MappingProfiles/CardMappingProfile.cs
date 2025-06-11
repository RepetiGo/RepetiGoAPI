using AutoMapper;

using FlashcardApp.Api.Dtos.CardDtos;

namespace FlashcardApp.Api.MappingProfiles
{
    public class CardMappingProfile : Profile
    {
        public CardMappingProfile()
        {
            CreateMap<CreateCardRequestDto, Card>();
            CreateMap<UpdateCardRequestDto, Card>();
            CreateMap<Card, CardResponseDto>();
        }
    }
}
