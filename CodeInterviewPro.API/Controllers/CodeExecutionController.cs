using CodeInterviewPro.Application.DTOs.CodeExecution;
using CodeInterviewPro.Application.DTOs.CodeExecutionRequest;
using CodeInterviewPro.Domain.Entities;
using CodeInterviewPro.Infrastructure.CodeExecution;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeExecutionController : ControllerBase
    {
        private readonly ExecutionPipelineService _pipeline;
        private readonly TestCaseExecutionService _testCaseService;
        private readonly CodeAnalysisService _analysisService;

        public CodeExecutionController(
            ExecutionPipelineService pipeline,
            TestCaseExecutionService testCaseService,
            CodeAnalysisService analysisService)
        {
            _pipeline = pipeline;
            _testCaseService = testCaseService;
            _analysisService = analysisService;
        }

        [HttpPost]
        public async Task<IActionResult> Run(
            [FromBody] CodeExecutionRequest request)
        {
            var result =
                await _pipeline.ExecuteAsync(
                    request.Code,
                    request.Language,
                    request.TestCases,
                    request.MethodName);

            return Ok(result);
        }

        [HttpPost("testcases")]
        public async Task<IActionResult> RunTestCases(
         [FromBody] TestCaseRequest request,
         CancellationToken token)
        {
            var result =
                await _testCaseService.ExecuteAsync(
                    request.Code,
                    request.Language,
                    request.TestCases,
                    request.MethodName,
                    token);

            return Ok(result);
        }
        [HttpPost("analyze")]
        public IActionResult Analyze(
            [FromBody] CodeExecutionRequest request)
        {
            var warnings =
                _analysisService.Analyze(request.Code);

            return Ok(warnings);
        }
    }
}