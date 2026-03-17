using CodeInterviewPro.API.Middleware;
using CodeInterviewPro.Application;
using CodeInterviewPro.Application.Common.Responses;
using CodeInterviewPro.Infrastructure;
using CodeInterviewPro.Infrastructure.Seed;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ================= AUTHENTICATION =================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Read token from cookies
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    };
});

// ================= AUTHORIZATION =================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",
        policy => policy.RequireClaim("rid", "1"));

    options.AddPolicy("HROnly",
        policy => policy.RequireClaim("rid", "2"));

    options.AddPolicy("CandidateOnly",
        policy => policy.RequireClaim("rid", "3"));
});

// ================= VALIDATION RESPONSE =================
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

// ================= CONTROLLERS =================
builder.Services.AddControllers();

// ================= FLUENT VALIDATION =================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// ================= APPLICATION + INFRA =================
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// ================= SWAGGER =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ================= SEED DATA =================
using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.Seed(scope.ServiceProvider);
}

// ================= MIDDLEWARE =================
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

app.Run();