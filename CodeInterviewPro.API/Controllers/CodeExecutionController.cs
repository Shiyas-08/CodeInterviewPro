using CodeInterviewPro.Application.DTOs.CodeExecutionRequest;
using CodeInterviewPro.Infrastructure.CodeExecution;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeExecutionController : ControllerBase
    {
        private readonly DockerCodeExecutionService _service;

        public CodeExecutionController(
            DockerCodeExecutionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Run(
            [FromBody] CodeExecutionRequest request)
        {
            var result =
                await _service.ExecuteAsync(request.Code);

            return Ok(result);
        }
    }
}