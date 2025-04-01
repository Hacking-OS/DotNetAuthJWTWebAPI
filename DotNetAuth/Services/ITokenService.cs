using DotNetAuth.Domain.Entities;

namespace DotNetAuth.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(ApplicationUser user);
        string GenerateRefreshToken();
    }
}
