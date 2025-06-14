using FlashcardApp.Api.Dtos.CardDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface ICardsService
    {
        Task<ServiceResult<ICollection<CardResponseDto>>> GetCardsByDeckIdAsync(int deckId, PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponseDto>> GetCardByIdAsync(int deckId, int cardId, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponseDto>> CreateCardAsync(int deckId, CreateCardRequestDto createCardDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<CardResponseDto>> UpdateCardAsync(int deckId, int cardId, UpdateCardRequestDto updateCardRequestDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> DeleteCardAsync(int deckId, int cardId, ClaimsPrincipal claimsPrincipal);
    }
}
