using RepetiGo.Api.Dtos.CardDtos;
using RepetiGo.Api.Dtos.ReviewDtos;
using RepetiGo.Api.Helpers;

namespace RepetiGo.Api.Interfaces.Services
{
    public interface IReviewsService
    {
        Task<ServiceResult<ICollection<CardResponse>>> GetDueCardsByDeckIdAsync(int deckId, PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponse>> ReviewCardAsync(int deckId, int cardId, ReviewRequest reviewRequest, ClaimsPrincipal claimsPrincipal);
    }
}
