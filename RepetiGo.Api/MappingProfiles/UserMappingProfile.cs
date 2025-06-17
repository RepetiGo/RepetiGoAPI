using AutoMapper;

using RepetiGo.Api.Dtos.ProfileDtos;

namespace RepetiGo.Api.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, ProfileResponse>();
            CreateMap<UpdateUsernameRequest, ApplicationUser>();
        }
    }
}
