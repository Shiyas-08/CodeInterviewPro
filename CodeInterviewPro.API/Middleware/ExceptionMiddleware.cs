using CodeInterviewPro.Application.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CodeInterviewPro.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";

                var statusCode = HttpStatusCode.InternalServerError;
                var message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred.";

                if (ex.Message.Contains("not found"))
                    statusCode = HttpStatusCode.NotFound;

                if (ex.Message.Contains("already exists"))
                    statusCode = HttpStatusCode.BadRequest;

                context.Response.StatusCode = (int)statusCode;

                var response = ApiResponse<string>.Failure(message);

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}