using STEMLabsServer.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services;
using STEMLabsServer.Services.Interfaces;

namespace STEMLabsServer.Controllers
{
    [Route("api/laboratories")]
    [ApiController]
    public class LaboratoryController(ILaboratoryService laboratoryService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateLaboratory([FromBody] LaboratoryDto laboratoryDto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }
            
            await laboratoryService.CreateLaboratory(laboratoryDto, cancellationToken);
            
            return Ok("Laboratory created successfully.");
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<LaboratoryFullListItemDto>>> GetLaboratories(CancellationToken cancellationToken)
        {
            var laboratories = await laboratoryService.GetLaboratories(cancellationToken);
            return Ok(laboratories);
        }
        
        [HttpGet("simplified")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<IdNameDto>>> GetLaboratoriesSimplified(CancellationToken cancellationToken)
        {
            var laboratories = await laboratoryService.GetLaboratoriesSimplified(cancellationToken);
            return Ok(laboratories);
        }
        
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LaboratoryDto>> GetLaboratory([FromRoute] int id, CancellationToken cancellationToken)
        {
            var laboratory = await laboratoryService.GetLaboratory(id, cancellationToken);
            if (laboratory == null)
            {
                return NotFound("Laboratory not found.");
            }
            
            return Ok(laboratory);
        }
        
        [HttpGet("{sceneId}/steps")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<string>>> GetLaboratorySteps([FromRoute] int sceneId, CancellationToken cancellationToken)
        {
            var steps = await laboratoryService.GetLaboratorySteps(sceneId, cancellationToken);
            if (!steps.Any())
            {
                return NotFound("No steps found for this laboratory.");
            }
            
            return Ok(steps);
        }
        
        [HttpGet("{id}/sessions")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<IdDateDto>>> GetLaboratorySessions([FromRoute] int id, CancellationToken cancellationToken)
        {
            var sessions = await laboratoryService.GetLaboratorySessions(id, cancellationToken);
            if (!sessions.Any())
            {
                return NotFound("No sessions found for this laboratory.");
            }
            
            return Ok(sessions);
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateLaboratory([FromRoute] int id, [FromBody] LaboratoryDto laboratoryDto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }
            
            await laboratoryService.UpdateLaboratory(id, laboratoryDto, cancellationToken);
            
            return Ok("Laboratory updated successfully.");
        }
    }
}
