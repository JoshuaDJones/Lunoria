using Eldoria.Api.Requests;
using Eldoria.Application.Dtos;
using Eldoria.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationTokenDto>> Register([FromBody] RegisterRequest req, CancellationToken ct)
        {
            var result = await _authService.RegisterUser(req.Email, req.Password, ct);

            if (result.Success)
                return Ok(result.Value);

            return result.Error!.Code switch
            {
                "User.InvalidEmail" => BadRequest(result.Error),
                "User.InvalidPassword" => BadRequest(result.Error),
                "User.EmailExists" => Conflict(result.Error),
                "User.RegisterFail" => StatusCode(500, result.Error),
                _ => BadRequest(result.Error)
            };
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationTokenDto>> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var result = await _authService.AuthenticateUser(req.Email, req.Password, ct);

            if (result.Success)
                return Ok(result.Value);

            if (result.Error!.Code == "User.LoginFailure")
                return Unauthorized(result.Error);

            return BadRequest(result.Error);
        }
    }
}
