using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{

    [Authorize(Policy = "AdminOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _repo;
        public AdminController(IAdminService repo)
        {
            _repo = repo;
        }
        [HttpPost("create-hr")]
        public async Task<ActionResult> CreateHr(CreateHrRequest request)
        {
            await _repo.CreateHr(request);
            return Ok(new
            {
                message = "Hr created successfully"
            });
        }
    }
}
