using AutoMapper;

namespace backend.MappingProfiles
{
    public class DeckMappingProfile : Profile
    {
        public DeckMappingProfile()
        {
            CreateMap<CreateDeckRequestDto, Deck>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
