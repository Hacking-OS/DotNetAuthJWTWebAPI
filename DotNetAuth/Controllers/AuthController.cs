using DotNetAuth.Domain.Contracts;
using DotNetAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAuth.Controllers
{
    [Route("api/")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var response = await _userService.RegisterAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
         [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var response = await _userService.LoginAsync(request);
            return Ok(response);
        }

        [HttpGet("user/{id}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> getUserByID(Guid id)
        {
            var response = await _userService.GetByIdAsync(id);
            return Ok(response);
        }


        [HttpPost("refresh-token")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _userService.RefreshTokenAsync(request);
            return Ok(response);
        }

        [HttpPost("revoke-refresh-token")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [Authorize]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _userService.RevokeRefreshToken(request);
            if(response != null && response.Message == "Refresh Token Revoked Successfully!")
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("current-user")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [Authorize]
        public async Task<IActionResult> CurrentUser()
        {
            var response = await _userService.GetCurrentUserAsync();
            return Ok(response);
        }


        [HttpPost("Sample")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public IActionResult GetSomething()
        {
            return NotFound(new ErrorResponse { Title = "Not Found", StatusCode = 404, Message = "Resource not found" });
        }
    }
}
