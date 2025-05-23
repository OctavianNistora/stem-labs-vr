using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services.Interfaces;

namespace STEMLabsServer.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> Register([FromBody] UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }
            
            var reasonForInvalid = await userService.RegisterUser(userRegisterDto, cancellationToken);
            if (!reasonForInvalid.Success)
            {
                return BadRequest(reasonForInvalid.FailureReason);
            }
            
            return Ok("User registered successfully.");
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserListItemDto>>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await userService.GetAllUsers(cancellationToken);

            return Ok(users);
        }
        
        [HttpGet("{id}/profile")]
        [Authorize]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }

            if (userId != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to view this user's profile.");
            }

            var userProfile = await userService.GetUserProfile(id, cancellationToken);
            
            if (userProfile == null)
            {
                return NotFound("User not found.");
            }

            return Ok(userProfile);
        }
        
        [HttpGet("{id}/email")]
        [Authorize]
        public async Task<ActionResult<string>> GetUserEmail([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }

            if (userId != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to view this user's email.");
            }

            var result = await userService.GetUserEmail(id, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok(result.Value);
        }
        
        [HttpPut("{id}/email")]
        [Authorize]
        public async Task<ActionResult<string>> UpdateUserEmail([FromRoute] int id, [FromBody] UserEmailUpdateDto userEmailUpdateDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }

            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }

            if (userId != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to update this user's email.");
            }

            var result = await userService.UpdateUserEmail(id, userEmailUpdateDto, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok("Email updated successfully.");
        }
        
        [HttpPut("{id}/password")]
        [Authorize]
        public async Task<ActionResult<string>> UpdateUserPassword([FromRoute] int id, [FromBody] UserPasswordUpdateDto userPasswordUpdateDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }

            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }

            if (userId != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to update this user's password.");
            }

            var result = await userService.UpdateUserPassword(id, userPasswordUpdateDto, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok("Password updated successfully.");
        }
        
        [HttpPut("{id}/profile")]
        [Authorize]
        public async Task<ActionResult<string>> UpdateUserProfile([FromRoute] int id, [FromBody] UserProfileDto userProfileUpdateDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }

            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }

            if (userId != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to update this user's profile.");
            }

            var result = await userService.UpdateUserProfile(id, userProfileUpdateDto, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok("User profile updated successfully.");
        }
        
        [HttpGet("{id}/related-laboratories")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RelatedLaboratoryDto>>> GetRelatedLaboratories([FromRoute] int id, CancellationToken cancellationToken)
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }

            if (userId != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to view related laboratories for this user.");
            }

            var laboratories = await userService.GetRelatedLaboratories(id, cancellationToken);
            
            if (laboratories == null)
            {
                return NotFound("User not found.");
            }
            if (!laboratories.Any())
            {
                return NotFound("No related laboratories found.");
            }

            return Ok(laboratories);
        }
        
        [HttpGet("{userId}/related-laboratories/{labId}/sessions")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RelatedSessionDto>>> GetRelatedSessions([FromRoute] int userId, [FromRoute] int labId, CancellationToken cancellationToken)
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var currentUserId))
            {
                return BadRequest("Invalid token user ID.");
            }

            if (currentUserId != userId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to view related sessions for this user.");
            }

            var sessions = await userService.GetRelatedSessions(userId, labId, cancellationToken);
            
            if (sessions == null)
            {
                return NotFound("User or laboratory not found.");
            }
            if (!sessions.Any())
            {
                return NotFound("No related sessions found.");
            }

            return Ok(sessions);
        }
        
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> UpdateUserRole([FromRoute] int id, [FromBody] string newRole, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }

            var result = await userService.UpdateUserRole(id, newRole, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok("User role updated successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> DeleteUser([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await userService.DeleteUser(id, cancellationToken);
            if (!result.Success)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok("User deleted successfully.");
        }
    }
}
