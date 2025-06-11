using FlashcardApp.Api.Dtos.DeckDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface ICardsService
    {
        Task<ServiceResult<ICollection<DeckResponseDto>>> GetDecksByUserIdAsync(ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponseDto>> GetDeckByIdAsync(int deckId, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponseDto>> CreateDeckAsync(CreateDeckRequestDto createDeckDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponseDto>> UpdateDeckAsync(int deckId, UpdateDeckRequestDto updateDeckRequestDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> DeleteDeckAsync(int deckId, ClaimsPrincipal claimsPrincipal);

    }
}
