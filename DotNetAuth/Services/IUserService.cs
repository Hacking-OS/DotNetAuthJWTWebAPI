using DotNetAuth.Domain.Contracts;

namespace DotNetAuth.Services
{
    public interface IUserService
    {
        Task<UserResponse> RegisterAsync(UserRegisterRequest request);
        Task<CurrentUserResponse> GetCurrentUserAsync();
        Task<UserResponse> GetByIdAsync(Guid id);
        Task<UserResponse> UpdateAsync(Guid id, UserUpdateRequest request);
        Task DeleteAsync(Guid id);
        Task<RevokedTokenResponse> RevokeRefreshToken(RefreshTokenRequest request);
        Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<UserResponse> LoginAsync(UserLoginRequest request);
    }
}
