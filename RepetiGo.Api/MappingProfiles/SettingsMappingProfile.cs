using AutoMapper;

using RepetiGo.Api.Dtos.SettingsDtos;

namespace RepetiGo.Api.MappingProfiles
{
    public class SettingsMappingProfile : Profile
    {
        public SettingsMappingProfile()
        {
            CreateMap<UpdateSettingsRequest, Settings>();
            CreateMap<Settings, SettingsResponse>();
        }
    }
}
