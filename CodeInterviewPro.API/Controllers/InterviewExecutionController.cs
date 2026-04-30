using CodeInterviewPro.Application.DTOs.Interview;
using CodeInterviewPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeInterviewPro.API.Controllers
{
    [ApiController]
    [Route("api/interview-execution")]
    public class InterviewExecutionController : ControllerBase
    {
        private readonly IInterviewExecutionService _service;

        public InterviewExecutionController(
            IInterviewExecutionService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("start")]
        public async Task<IActionResult> Start(
            [FromBody] StartInterviewRequest request)
        {
            var result = await _service.StartInterviewAsync(request.Token);

            return Ok(result);
        }


        [HttpPost("run")]
        public async Task<IActionResult> Run(
    [FromBody] SubmitCodeRequest request)
        {
            var result =
                await _service.RunCodeAsync(request);

            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPost("submit")]
        public async Task<IActionResult> Submit(
            [FromBody] SubmitCodeRequest request)
        {
            var result = await _service.SubmitCodeAsync(request);

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("run")]
        public async Task<IActionResult> RunCode(
     [FromBody] SubmitCodeRequest request)
        {
            var result = await _service.RunCodeAsync(request);
            return Ok(result);
        }
    }
}
//using CodeInterviewPro.Application.DTOs.Interview;
//using CodeInterviewPro.Application.Interfaces.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace CodeInterviewPro.API.Controllers
//{
//    [ApiController]
//    [Route("api/interview-execution")]
//    public class InterviewExecutionController : ControllerBase
//    {
//        private readonly IInterviewExecutionService _service;

//        public InterviewExecutionController(
//            IInterviewExecutionService service)
//        {
//            _service = service;
//        }
//        [AllowAnonymous]
//        [HttpPost("join")]
//        public async Task<IActionResult> JoinInterview(
//            [FromBody] JoinInterviewRequest request)
//        {
//            var result = await _service.JoinInterviewAsync(request.Token);

//            return Ok(result);
//        }
//        [AllowAnonymous]
//        [HttpPost("start")]
//        public async Task<IActionResult> Start(
//       StartInterviewRequest request)
//        {
//            var result = await _service.StartInterviewAsync(request.Token);

//            return Ok(result);
//        }

//        [AllowAnonymous]
//        [HttpPost("questions")]
//        public async Task<IActionResult> GetQuestions(GetQuestionsRequest request)
//        {
//            var result = await _service.GetQuestionsAsync(request.Token);

//            return Ok(result);
//        }
//        [AllowAnonymous]
//        [HttpPost("submit")]
//        public async Task<IActionResult> Submit([FromBody] SubmitCodeRequest request)
//        {
//            var result = await _service.SubmitCodeAsync(request);

//            return Ok(result);
//        }
//    }
//}