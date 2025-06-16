using AutoMapper;

using RepetiGo.Api.Dtos.CardDtos;
using RepetiGo.Api.Models;

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
