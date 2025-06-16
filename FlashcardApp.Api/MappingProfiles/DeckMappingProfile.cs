using AutoMapper;

using RepetiGo.Api.Dtos.DeckDtos;
using RepetiGo.Api.Models;

namespace RepetiGo.Api.MappingProfiles
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
