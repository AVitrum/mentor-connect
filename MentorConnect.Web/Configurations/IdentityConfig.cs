using System.Text;
using MentorConnect.Data.Contexts;
using MentorConnect.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MentorConnect.Web.Configurations;

public static class IdentityConfig
{
    public static void AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
                options.SaveTokens = true;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:IssuerSigningKey"] ?? string.Empty))
                };
            });
    }
}