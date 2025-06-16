using AutoMapper;

using FlashcardApp.Api.Dtos.ProfileDtos;

namespace FlashcardApp.Api.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, ProfileResponse>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Decks, opt => opt.Ignore());

            CreateMap<UpdateUsernameRequest, ApplicationUser>();
        }
    }
}
