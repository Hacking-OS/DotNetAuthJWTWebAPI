using Microsoft.AspNetCore.Identity;

namespace DotNetAuth.Domain.Contracts
{
    public class UserRegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
    }

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

    }

    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CurrentUserResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? AccessToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserUpdateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
    }



    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class RevokedTokenResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }   
}
