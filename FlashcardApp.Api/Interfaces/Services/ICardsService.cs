using FlashcardApp.Api.Dtos.CardDtos;
using FlashcardApp.Api.Dtos.GeneratedCardDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface ICardsService
    {
        Task<ServiceResult<ICollection<CardResponse>>> GetCardsByDeckIdAsync(int deckId, PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponse>> GetCardByIdAsync(int deckId, int cardId, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponse>> CreateCardAsync(int deckId, CreateCardRequest createCard, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponse>> UpdateCardAsync(int deckId, int cardId, UpdateCardRequest updateCardRequest, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> DeleteCardAsync(int deckId, int cardId, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<PreviewCardResponse>> GenerateCardAsync(GenerateRequest generateRequest, ClaimsPrincipal claimsPrincipal);
    }
}
