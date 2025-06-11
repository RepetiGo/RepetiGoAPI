using AutoMapper;

using backend.Dtos.UserDtos;

namespace backend.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, ProfileResponseDto>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }
    }
}
