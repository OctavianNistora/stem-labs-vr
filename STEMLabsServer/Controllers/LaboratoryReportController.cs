using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services;
using STEMLabsServer.Services.Interfaces;
using STEMLabsServer.Shared;

namespace STEMLabsServer.Controllers
{
    [Route("api/laboratory-reports")]
    [ApiController]
    public class LaboratoryReportController(ILaboratoryReportService laboratoryReportService) : ControllerBase
    {
        [HttpPost, RequestSizeLimit(5*1024*1024)]
        [Authorize]
        public async Task<ActionResult<string>> AddLaboratoryReport([FromForm] SubmittedLaboratoryReportDto submittedLaboratoryReportDto, CancellationToken cancellationToken)
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

            if (userId != submittedLaboratoryReportDto.submitterId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "You are not authorized to add a laboratory report for this user.");
            }

            var result = await laboratoryReportService.AddLaboratoryReport(submittedLaboratoryReportDto, cancellationToken);
            if (!result)
            {
                return BadRequest("Failed to add laboratory report.");
            }

            return Ok("Laboratory report added successfully.");
        }

        [HttpGet("{reportId}")]
        [Authorize]
        public async Task<ActionResult<DetailedLaboratoryReportDto>> GetLaboratoryReport([FromRoute] int reportId,
            CancellationToken cancellationToken)
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
            var sessionCreatorId =
                await laboratoryReportService.GetLaboratorySessionCreatorId(reportId, cancellationToken);
            var reportSubmitterId =
                await laboratoryReportService.GetLaboratoryReportSubmitterId(reportId, cancellationToken);
            if (userRole != nameof(UserRole.Admin) && userId != sessionCreatorId && userId != reportSubmitterId)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    "You are not authorized to view this laboratory report.");
            }

            var laboratoryReport = await laboratoryReportService.GetLaboratoryReport(reportId, cancellationToken);
            if (laboratoryReport == null)
            {
                return NotFound("Laboratory report not found.");
            }

            return Ok(laboratoryReport);
        }
    }
}
