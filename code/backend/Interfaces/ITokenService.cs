namespace backend.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IdentityUser user);
    }
}
