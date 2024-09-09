using MentorConnect.Data.Interfaces;
using MentorConnect.Web.Interfaces;
using MentorConnect.Web.Middlewares;
using MentorConnect.Web.Services;
using Microsoft.AspNetCore.Diagnostics;

// using MentorConnect.Data.Repositories;

namespace MentorConnect.Web.Configurations;

public static class DependencyInjectionExtensions
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        
    }
    
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IGoogleAuthService, GoogleAuthService>().AddProblemDetails().AddExceptionHandler<ExceptionHandlingMiddleware>();
        services.AddScoped<IEmailService, EmailService>().AddProblemDetails().AddExceptionHandler<ExceptionHandlingMiddleware>();
    }
}