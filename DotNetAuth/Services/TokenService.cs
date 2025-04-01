using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotNetAuth.Domain.Contracts;
using DotNetAuth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DotNetAuth.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _SecretKey;
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly double _expires;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ILogger<TokenService> logger)
        {   
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if(jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new ArgumentNullException(nameof(jwtSettings));
            }
            _SecretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            _validIssuer = jwtSettings.ValidIssuer;
            _validAudience = jwtSettings.ValidAudience;
            _expires = Convert.ToDouble(jwtSettings.Expires);
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> CreateToken(ApplicationUser user)
        {
           var SigningCredentials = new SigningCredentials(_SecretKey, SecurityAlgorithms.HmacSha256);
           var claims = await GetClaimsAsync(user);
           var tokenOptions = GenerateTokenOptions(SigningCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user?.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
                new Claim("FirstName", user?.FirstName ?? string.Empty),
                new Claim("LastName", user?.LastName ?? string.Empty),
                new Claim("Gender", user?.Gender ?? string.Empty)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {

            return new JwtSecurityToken(
                issuer:_validIssuer,
                audience:_validAudience,
                claims:claims,
                expires: DateTime.Now.AddMinutes(_expires),
                signingCredentials: signingCredentials
            );
        }

        public string GenerateRefreshToken()
        {
           var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
