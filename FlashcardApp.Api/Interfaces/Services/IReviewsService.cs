using FlashcardApp.Api.Dtos.CardDtos;
using FlashcardApp.Api.Dtos.ReviewDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IReviewsService
    {
        Task<ServiceResult<ICollection<CardResponse>>> GetDueCardsByDeckIdAsync(int deckId, PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponse>> ReviewCardAsync(int deckId, int cardId, ReviewRequest reviewRequest, ClaimsPrincipal claimsPrincipal);
    }
}
