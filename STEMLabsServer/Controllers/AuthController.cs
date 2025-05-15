using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services;

namespace STEMLabsServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("session")]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDto userLoginDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }
            
            var token = await authService.LoginAsync(userLoginDto,cancellationToken);
            if (token == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            
            return Ok(token);
        }
        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetUserInfo(CancellationToken cancellationToken)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                foreach (var claim in claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                var s = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine(s);
            }
            return Ok();
        }
        
        [HttpPost("test"), RequestSizeLimit(5*1024*1024)]
        public async Task<ActionResult> Test([FromForm] LaboratoryReportDto file)
        {
            return Ok();
        }
    }
}
