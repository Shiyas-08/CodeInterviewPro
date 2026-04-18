using CodeInterviewPro.API.Hubs;
using CodeInterviewPro.API.Middleware;
using CodeInterviewPro.Application;
using CodeInterviewPro.Application.Common.Responses;
using CodeInterviewPro.Infrastructure;
using CodeInterviewPro.Infrastructure.Cache;
using CodeInterviewPro.Infrastructure.Seed;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Authentication
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
//Console.WriteLine($"JWT KEY: {jwtKey}");

if (string.IsNullOrEmpty(jwtKey))
{
throw new Exception("JWT key not found in environment variables");
}

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(token))
                context.Token = token;

            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<string>.Failure("Unauthorized");

            return context.Response.WriteAsJsonAsync(response);
        },

        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<string>.Failure("Forbidden");

            return context.Response.WriteAsJsonAsync(response);
        }
    };
});

// Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireClaim("rid", "1"));

    options.AddPolicy("HROnly",
        policy => policy.RequireClaim("rid", "2"));

    options.AddPolicy("CandidateOnly",
        policy => policy.RequireClaim("rid", "3"));
});

// Redis

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"));

//builder.Services.AddScoped<RedisService>();

// Validation Response

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var response = ApiResponse<List<string>>.Failure("Validation failed");
        response.Data = errors;

        return new BadRequestObjectResult(response);
    };
});

// Services

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddScoped<DapperContext>();

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var context = sp.GetRequiredService<DapperContext>();
    return context.CreateConnection();
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
System.Net.ServicePointManager.SecurityProtocol =
    System.Net.SecurityProtocolType.Tls12 |
    System.Net.SecurityProtocolType.Tls13;


var app = builder.Build();

// Seed

using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.Seed(scope.ServiceProvider);
}
app.UseCors("AllowFrontend");

// Middleware

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<InterviewHub>("/interviewHub");

app.Run();