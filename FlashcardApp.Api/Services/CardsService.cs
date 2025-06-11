
using FlashcardApp.Api.Dtos.DeckDtos;

namespace FlashcardApp.Api.Services
{
    public class CardsService : ICardsService
    {
        public Task<ServiceResult<DeckResponseDto>> CreateDeckAsync(CreateDeckRequestDto createDeckDto, ClaimsPrincipal claimsPrincipal)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<object>> DeleteDeckAsync(int deckId, ClaimsPrincipal claimsPrincipal)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<DeckResponseDto>> GetDeckByIdAsync(int deckId, ClaimsPrincipal claimsPrincipal)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<ICollection<DeckResponseDto>>> GetDecksByUserIdAsync(ClaimsPrincipal claimsPrincipal)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<DeckResponseDto>> UpdateDeckAsync(int deckId, UpdateDeckRequestDto updateDeckRequestDto, ClaimsPrincipal claimsPrincipal)
        {
            throw new NotImplementedException();
        }
    }
}
