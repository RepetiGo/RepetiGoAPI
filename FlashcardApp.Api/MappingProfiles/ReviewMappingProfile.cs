using AutoMapper;

using FlashcardApp.Api.Dtos.ReviewDtos;

namespace FlashcardApp.Api.MappingProfiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<Card, DueCardResponseDto>();
        }
    }
}
