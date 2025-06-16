using FlashcardApp.Api.Dtos.DeckDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IDecksService
    {
        Task<ServiceResult<ICollection<DeckResponse>>> GetDecksByUserIdAsync(PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponse>> GetDeckByIdAsync(int deckId, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponse>> CreateDeckAsync(CreateDeckRequest createDeck, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponse>> UpdateDeckAsync(int deckId, UpdateDeckRequest updateDeckRequest, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> DeleteDeckAsync(int deckId, ClaimsPrincipal claimsPrincipal);
    }
}
