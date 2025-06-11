namespace backend.Interfaces.Services
{
    public interface IDecksService
    {
        Task<ServiceResult<ICollection<Deck>>> GetDecksByUserIdAsync(ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<Deck>> GetDeckByIdAsync(int deckId, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<Deck>> CreateDeckAsync(CreateDeckRequestDto createDeckDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<Deck>> UpdateDeckAsync(UpdateDeckRequestDto updateDeckDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> DeleteDeckAsync(int deckId, ClaimsPrincipal claimsPrincipal);
    }
}
