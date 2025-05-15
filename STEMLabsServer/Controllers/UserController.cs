using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services;

namespace STEMLabsServer.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<string>> Register([FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }
            
            var result = await userService.RegisterUser(userRegisterDto, cancellationToken);
            if (!result)
            {
                return BadRequest("User already exists.");
            }
            
            return Ok("User registered successfully.");
        }
        
        [HttpPost("{id}/laboratory-sessions")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<string>> AddLaboratorySession([FromRoute] int id, [FromBody] LaboratorySessionDto laboratorySessionDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }

            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }
            
            if (userId != id)
            {
                return Forbid("You are not authorized to add a laboratory session for this user.");
            }
            
            var result = await userService.AddLaboratorySession(id, laboratorySessionDto, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to add laboratory session.");
            }
            
            return Ok("Laboratory session added successfully.");
        }

        [HttpPost("{id}/laboratory-report")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<string>> AddLaboratoryReport([FromRoute] int id,
            [FromBody] LaboratoryReportDto laboratoryReportDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }

            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }

            if (userId != id)
            {
                return Forbid("You are not authorized to add a laboratory report for this user.");
            }

            var result = await userService.AddLaboratoryReport(id, laboratoryReportDto, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to add laboratory report.");
            }

            return Ok("Laboratory report added successfully.");
        }
    }
}
