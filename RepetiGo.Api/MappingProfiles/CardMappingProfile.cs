using AutoMapper;

using RepetiGo.Api.Dtos.CardDtos;

namespace RepetiGo.Api.MappingProfiles
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
