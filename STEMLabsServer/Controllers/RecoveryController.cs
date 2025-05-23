using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Services;
using STEMLabsServer.Services.Interfaces;

namespace STEMLabsServer.Controllers
{
    [Route("api/recovery")]
    [ApiController]
    public class RecoveryController(IRecoveryService recoveryService) : ControllerBase
    {
        [HttpPost("username-reminder")]
        public async Task<ActionResult> RemindUsername([FromBody] string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            var result = await recoveryService.RemindUsernameAsync(email, cancellationToken);
            if (!result)
            {
                return NotFound("Email not found.");
            }

            return Ok("Username reminder sent successfully. Please check your email.");
        }

        [HttpPost("password-request")]
        public async Task<ActionResult> RequestPasswordReset([FromBody] string username,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Username is required.");
            }

            var result = await recoveryService.RequestPasswordResetAsync(username, cancellationToken);
            if (!result)
            {
                return NotFound("Username not found.");
            }

            return Ok("Password reset request successful. Please check your email.");
        }

        [HttpPost("password-reset")]
        public async Task<ActionResult> ResetPassword([FromBody] RecoveryPasswordResetDto recoveryPasswordResetDto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid body format.");
            }

            var result = await recoveryService.ResetPasswordAsync(recoveryPasswordResetDto, cancellationToken);
            if (!result)
            {
                return NotFound("Invalid token.");
            }

            return Ok("Password reset successful.");
        }
    }
}