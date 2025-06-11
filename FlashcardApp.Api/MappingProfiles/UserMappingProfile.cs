using AutoMapper;

using FlashcardApp.Api.Dtos.UserDtos;

namespace FlashcardApp.Api.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, ProfileResponseDto>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Decks, opt => opt.Ignore())
                .ForMember(dest => dest.Settings, opt => opt.Ignore());
        }
    }
}
