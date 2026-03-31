using CodeInterviewPro.Application.DTOs.CodeExecution;
using CodeInterviewPro.Application.DTOs.CodeExecutionRequest;
using CodeInterviewPro.Infrastructure.CodeExecution;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeExecutionController : ControllerBase
    {
        private readonly MultiLanguageExecutionService _service;
        private readonly TestCaseExecutionService _testCaseService;

        public CodeExecutionController(
            MultiLanguageExecutionService service, TestCaseExecutionService testCaseService)
        {
            _service = service;
            _testCaseService = testCaseService;
        }

        [HttpPost]
        public async Task<IActionResult> Run(
            [FromBody] CodeExecutionRequest request)
        {
            var result =
                await _service.ExecuteAsync(
                    request.Code,
                    request.Language);

            return Ok(result);
        }
        [HttpPost("testcases")]
        public async Task<IActionResult> RunTestCases(
    [FromBody] TestCaseRequest request)
        {
            var result = await _testCaseService.ExecuteAsync(
                    request.Code,
                    request.Language,
                    request.TestCases);

            return Ok(result);
        }
    }
}