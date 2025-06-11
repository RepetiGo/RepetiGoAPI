using AutoMapper;

namespace backend.Services
{
    public class DecksService : IDecksService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DecksService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ICollection<Deck>>> GetDecksByUserIdAsync(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ICollection<Deck>>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<ICollection<Deck>>.Failure(
                    "User not found",
                    HttpStatusCode.NotFound
                );
            }

            var decks = await _unitOfWork.DecksRepository.GetAllAsync(
                filter: d => d.UserId == userId,
                orderBy: q => q.OrderBy(d => d.CreatedAt),
                includeProperties: "Cards"
            );

            return ServiceResult<ICollection<Deck>>.Success(decks);
        }

        public async Task<ServiceResult<Deck>> GetDeckByIdAsync(int deckId, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<Deck>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<Deck>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<Deck>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            return ServiceResult<Deck>.Success(deck);
        }

        public async Task<ServiceResult<Deck>> CreateDeckAsync(CreateDeckRequestDto createDeckDto, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<Deck>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = new Deck
            {
                Name = createDeckDto.Name,
                UserId = userId,
            };

            await _unitOfWork.DecksRepository.AddAsync(deck);
            await _unitOfWork.SaveAsync();
            return ServiceResult<Deck>.Success(deck);
        }

        public async Task<ServiceResult<Deck>> UpdateDeckAsync(UpdateDeckRequestDto updateDeckDto, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<Deck>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            if (updateDeckDto.UserId != userId)
            {
                return ServiceResult<Deck>.Failure(
                    "You do not have permission to update this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(updateDeckDto.Id);
        }

        public async Task<ServiceResult<object>> DeleteDeckAsync(int deckId, ClaimsPrincipal claimsPrincipal)
        {
        }
    }
}
