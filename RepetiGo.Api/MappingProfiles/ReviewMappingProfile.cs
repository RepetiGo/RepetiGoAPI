using AutoMapper;

using RepetiGo.Api.Dtos.ReviewDtos;

namespace RepetiGo.Api.MappingProfiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<Card, ReviewResponse>();
        }
    }
}
