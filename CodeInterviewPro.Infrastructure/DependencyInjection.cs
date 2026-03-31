using CodeInterviewPro.Application.Interfaces.Repositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewRepositories;
using CodeInterviewPro.Application.Interfaces.Repositories.InterviewsRepositories;
using CodeInterviewPro.Application.Interfaces.Services;
using CodeInterviewPro.Domain.Common.Interfaces;
using CodeInterviewPro.Infrastructure.CodeExecution;
using CodeInterviewPro.Infrastructure.CodeExecution.Executors;
using CodeInterviewPro.Infrastructure.Identity;
using CodeInterviewPro.Infrastructure.Repositories;
using CodeInterviewPro.Infrastructure.Repositories.InterviewRepositories;
using CodeInterviewPro.Infrastructure.Repositories.QuestionRepositories;
using CodeInterviewPro.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            //register services 
            services.AddScoped<DapperContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher,PasswordHasher>();
            services.AddScoped<JwtService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IInterviewRepository, InterviewRepository>();
            services.AddScoped<IInterviewCandidateRepository, InterviewCandidateRepository>();
            services.AddScoped<IInterviewInvitationRepository, InterviewInvitationRepository>();
            services.AddScoped<IInterviewSessionRepository, InterviewSessionRepository>();
            services.AddScoped<IInterviewQuestionRepository, InterviewQuestionRepository>();
            services.AddScoped< IInterviewSubmissionRepository,InterviewSubmissionRepository>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<CodeExecutionService>();
            services.AddScoped<DockerCodeExecutionService>();

            //code excution
            services.AddScoped<CSharpExecutor>();
            services.AddScoped<PythonExecutor>();
            services.AddScoped<NodeExecutor>();
            services.AddScoped<JavaExecutor>();
            services.AddScoped<GoExecutor>();
            services.AddScoped<MultiLanguageExecutionService>(); 

            services.AddScoped<ILanguageExecutionFactory, LanguageExecutionFactory>();
            services.AddScoped<TestCaseExecutionService>(); 
            services.AddScoped<CodeAnalysisService>();

            return services;


        }
    }
}
