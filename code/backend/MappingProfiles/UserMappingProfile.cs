using AutoMapper;

namespace backend.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<ApplicationUser, ProfileDto>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
