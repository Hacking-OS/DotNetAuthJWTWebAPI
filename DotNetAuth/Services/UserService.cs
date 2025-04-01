using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using DotNetAuth.Domain.Contracts;
using DotNetAuth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotNetAuth.Services
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(ITokenService tokenService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, ILogger<UserService> logger, IMapper mapper)
        {
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
        {
            _logger.LogInformation("Registering user");
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogInformation("Email exist");
                throw new Exception("Email exist");
            }
            var user = _mapper.Map<ApplicationUser>(request);

            user.UserName = GetUserName(request.FirstName, request.LastName);
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogInformation("Error creating user: {errors}", errors);
                throw new Exception("Error creating user:   \n" + errors);
            }
            _logger.LogInformation("User created");
            await _tokenService.CreateToken(user);
            //user.RefreshToken = await _tokenService.CreateToken(user);
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserResponse> LoginAsync(UserLoginRequest request)
        {
            if (request == null)
            {
                _logger.LogInformation("Request is null");
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }
            var token = await _tokenService.CreateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            using var sha256 = SHA256.Create();
            var hashedRefreshToken = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken)));
            user.RefreshToken = hashedRefreshToken;
            user.RefreshTokenExpiryDate = DateTime.Now.AddDays(2);
            user.UpdatedAt = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error updating user: {errors}", errors);
                throw new Exception("Error updating user: {errors}");
            }
            var UserResponse = _mapper.Map<ApplicationUser,UserResponse>(user);
            UserResponse.RefreshToken = refreshToken;
            UserResponse.AccessToken = token;
            return UserResponse;

        }

   



        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }
            return _mapper.Map<UserResponse>(user);
        }

        public async Task DeleteAsync(Guid id)
        {
           var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error deleting user: {errors}", errors);
                throw new Exception("Error deleting user: {errors}");
            }
        }


        public async Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            _logger.LogInformation("Refreshing token");
            using var sha256 = SHA256.Create();
            var hashedRefreshToken = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken)));
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);
            if (user == null || user.RefreshTokenExpiryDate < DateTime.Now)
            {
                _logger.LogInformation("Invalid token");
                throw new Exception("Invalid token");
            }

            var token = await _tokenService.CreateToken(user);
            _logger.LogInformation("Token refreshed");
            var currentUserResponse = _mapper.Map<CurrentUserResponse>(user);
            currentUserResponse.AccessToken = token;
            return currentUserResponse;
        }




        public async Task<RevokedTokenResponse> RevokeRefreshToken(RefreshTokenRequest request)
        {
          _logger.LogInformation("Revoking token");
            using var sha256 = SHA256.Create();
            var hashedRefreshToken = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken)));
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);
            if (user == null)
            {
                _logger.LogInformation("Invalid token");
                throw new Exception("Invalid token");
            }

            if(user.RefreshTokenExpiryDate < DateTime.Now)
            {
                _logger.LogInformation("Token expired");
                throw new Exception("Token expired");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryDate = null;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error updating user: {errors}", errors);
                throw new Exception("Error updating user: {errors}");
            }
            _logger.LogInformation("Token revoked");
            return new RevokedTokenResponse {
                Token = request.RefreshToken,
                Message = "Refresh Token Revoked Successfully!"
            };
        }




        public async Task<UserResponse> UpdateAsync(Guid id, UserUpdateRequest request)
        {
          var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.Gender = request.Gender;
                user.UpdatedAt = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Error updating user: {errors}", errors);
                throw new Exception("Error updating user: {errors}");
            }
            return _mapper.Map<UserResponse>(user);
        }


        private string GetUserName(string firstName, string lastName)
        {
            var BaseName = $"{firstName}{lastName}".ToLower();
            var UserName = BaseName;
            var count = 1;
            while (_userManager.Users.Any(u => u.UserName == UserName))
            {
                UserName = $"{BaseName}{count}";
                count++;
            }
            return UserName;
        }

        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            var user = await _userManager.FindByIdAsync(_currentUserService?.getUserId() ?? string.Empty);
            if (user == null)
            {
                _logger.LogInformation("User not found");
                throw new Exception("User not found");
            }
            return _mapper.Map<CurrentUserResponse>(user);
        }

    }
}
