using CodeInterviewPro.Application.Common.Responses;
using CodeInterviewPro.Application.DTOs;
using CodeInterviewPro.Application.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    using CodeInterviewPro.Application.Interfaces.Repositories;
    using CodeInterviewPro.Domain.Entities;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/tenants")]
    [Authorize(Policy = "AdminOnly")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _service;

        public TenantController(ITenantService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTenantRequest request)
        {
            await _service.CreateAsync(request);

            return Ok(ApiResponse<string>
                .SuccessResponse(null, "Tenant created"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenants = await _service.GetAllAsync();

            return Ok(ApiResponse<IEnumerable<Tenant>>
                .SuccessResponse(tenants, "Tenant list"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tenant = await _service.GetByIdAsync(id);

            if (tenant == null)
                return NotFound(ApiResponse<string>.Failure("Not found"));

            return Ok(ApiResponse<Tenant>
                .SuccessResponse(tenant, "Tenant details"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateTenantRequest request)
        {
            await _service.UpdateAsync(id, request);

            return Ok(ApiResponse<string>
                .SuccessResponse(null, "Tenant updated"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);

            return Ok(ApiResponse<string>
                .SuccessResponse(null, "Tenant deactivated"));
        }
    }
}