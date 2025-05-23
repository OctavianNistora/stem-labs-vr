using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services;
using STEMLabsServer.Services.Interfaces;

namespace STEMLabsServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("session")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto userLoginDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }
            
            var authData = await authService.LoginAsync(userLoginDto,cancellationToken);
            if (authData == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            
            return Ok(authData);
        }
        
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }
            
            var token = await authService.GenerateAuthDataFromRefreshToken(refreshToken, cancellationToken);
            if (token == null)
            {
                return Unauthorized("Invalid refresh token.");
            }
            
            return Ok(token);
        }

        [HttpGet]
        [Authorize]
        public Task<ActionResult<AuthResponseDto>> Test(CancellationToken cancellationToken)
        {
            return Task.FromResult<ActionResult<AuthResponseDto>>(Ok());
        }
    }
}
