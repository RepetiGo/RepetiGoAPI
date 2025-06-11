namespace FlashcardApp.Api.Helpers
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return string.Empty;
            }

            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
    }
}
