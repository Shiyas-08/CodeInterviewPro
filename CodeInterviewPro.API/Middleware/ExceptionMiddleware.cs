using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
namespace CodeInterviewPro.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unhandled exception occurred.");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var responce = new
                {
                    message= "An unexpected error occurred.",
                    statusCode=context.Response.StatusCode,
                    time=DateTime.UtcNow
                };
                var json = JsonSerializer.Serialize(responce);
                await context.Response.WriteAsync(json);
            }
        }
        } 
    }

