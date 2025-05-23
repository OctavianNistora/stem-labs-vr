using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services.Interfaces;
using STEMLabsServer.Shared;

namespace STEMLabsServer.Controllers
{
    [Route("api/laboratory-sessions")]
    [ApiController]
    public class LaboratorySessionController(ILaboratorySessionService laboratorySessionService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Professor,Admin")]
        public async Task<ActionResult<string>> AddLaboratorySession(
            [FromBody] LaboratorySessionDto laboratorySessionDto, CancellationToken cancellationToken)
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
            
            if (userId != laboratorySessionDto.creatorId)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    "You are not authorized to add a laboratory session for this user.");
            }

            var result = await laboratorySessionService.AddLaboratorySession(laboratorySessionDto, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to add laboratory session.");
            }
            
            return Ok("Laboratory session added successfully.");
        }

        [HttpGet("{sessionId}/participants")]
        [Authorize(Roles = "Professor,Admin")]
        public async Task<ActionResult<IEnumerable<LaboratorySessionParticipantDto>>> GetParticipants(
            [FromRoute] int sessionId, CancellationToken cancellationToken)
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return BadRequest("Invalid token user ID.");
            }

            var userRole = identity.FindFirst(ClaimTypes.Role)?.Value;
            
            var creatorId = await laboratorySessionService.GetLaboratorySessionCreator(sessionId, cancellationToken);
            if (userRole != nameof(UserRole.Admin) && userId != creatorId)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    "You are not authorized to view this session's participants.");
            }

            var participants =
                await laboratorySessionService.GetLaboratorySessionParticipants(sessionId, cancellationToken);

            if (!participants.Any())
            {
                return NotFound("No participants found for this session.");
            }

            return Ok(participants);
        }

        [HttpGet("{sessionId}/participants/{participantId}/reports")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LaboratoryReportListItemDto>>> GetParticipantReports(
            [FromRoute] int sessionId, [FromRoute] int participantId, CancellationToken cancellationToken)
        {
            if (HttpContext.User.Identity is not ClaimsIdentity identity)
            {
                return Unauthorized("Token is missing claims.");
            }

            if (!int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var currentUserId))
            {
                return BadRequest("Invalid token user ID.");
            }

            var currentUserRole = identity.FindFirst(ClaimTypes.Role)?.Value;

            var creatorId = await laboratorySessionService.GetLaboratorySessionCreator(sessionId, cancellationToken);
            if (currentUserRole != nameof(UserRole.Admin) && currentUserId != creatorId && currentUserId != participantId)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    "You are not authorized to view these reports.");
            }

            var reports =
                await laboratorySessionService.GetParticipantReports(sessionId, participantId, cancellationToken);
            if (!reports.Any())
            {
                return NotFound("No reports found.");
            }

            return Ok(reports);
        }
    }
}
