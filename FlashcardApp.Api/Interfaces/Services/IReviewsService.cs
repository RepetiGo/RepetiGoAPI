using FlashcardApp.Api.Dtos.CardDtos;
using FlashcardApp.Api.Dtos.ReviewDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IReviewsService
    {
        Task<ServiceResult<ICollection<CardResponseDto>>> GetDueCardsByDeckIdAsync(int deckId, PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponseDto>> ReviewCardAsync(int deckId, int cardId, ReviewRequestDto reviewRequestDto, ClaimsPrincipal claimsPrincipal);
    }
}
