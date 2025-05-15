using STEMLabsServer.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services;

namespace STEMLabsServer.Controllers
{
    [Route("api/laboratories")]
    [ApiController]
    public class LaboratoryController(ILabService labService) : ControllerBase
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
            
            await labService.CreateLaboratory(laboratoryDto, cancellationToken);
            
            return Ok("Laboratory created successfully.");
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
            
            await labService.UpdateLaboratory(id, laboratoryDto, cancellationToken);
            
            return Ok("Laboratory updated successfully.");
        }
    }
}
