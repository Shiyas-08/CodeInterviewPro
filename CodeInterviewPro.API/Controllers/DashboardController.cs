using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"CLAIM: {claim.Type} = {claim.Value}");
            }

            var role = User.Claims.FirstOrDefault(c => c.Type == "rid")?.Value;
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "uid").Value);

            var tenantClaim = User.Claims.FirstOrDefault(c => c.Type == "tid");

            if (role == "2" && tenantClaim == null)
                return BadRequest("TenantId missing");

            var tenantId = tenantClaim != null ? Guid.Parse(tenantClaim.Value) : Guid.Empty;

            var result = await _service.GetSummaryAsync(role!, userId, tenantId);

            return Ok(result);
        }
        [HttpGet("insights")]
        public async Task<IActionResult> GetInsights()
        {
            var role = User.FindFirst("rid")?.Value;
            var userClaim = User.FindFirst("uid");

            if (string.IsNullOrEmpty(role) || userClaim == null)
                return Unauthorized("Invalid token");

            var userId = Guid.Parse(userClaim.Value);

            Guid tenantId = Guid.Empty;

            //  Only HR needs tenant
            if (role == "2")
            {
                var tenantClaim = User.FindFirst("tid");

                if (tenantClaim == null)
                    return BadRequest("TenantId missing");

                tenantId = Guid.Parse(tenantClaim.Value);
            }

            var result = await _service.GetInsightsAsync(role, userId, tenantId);

            return Ok(result);
        }
    }
}
