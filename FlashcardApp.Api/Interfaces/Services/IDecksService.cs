using FlashcardApp.Api.Dtos.DeckDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IDecksService
    {
        Task<ServiceResult<ICollection<DeckResponseDto>>> GetDecksByUserIdAsync(PaginationQuery paginationQuery, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponseDto>> GetDeckByIdAsync(int deckId, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponseDto>> CreateDeckAsync(CreateDeckRequestDto createDeckDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<DeckResponseDto>> UpdateDeckAsync(int deckId, UpdateDeckRequestDto updateDeckRequestDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> DeleteDeckAsync(int deckId, ClaimsPrincipal claimsPrincipal);
    }
}
