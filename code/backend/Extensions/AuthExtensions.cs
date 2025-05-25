using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace backend.Extensions
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                // Configure the way we validate the received token
                options.SaveToken = true; // Save the token in HttpContext
                options.RequireHttpsMetadata = false; // Enforce HTTPS in production
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetValue<string>("Jwt:Issuer") ?? throw new InvalidOperationException("Issuer not found"),
                    ValidAudience = configuration.GetValue<string>("Jwt:Audience") ?? throw new InvalidOperationException("Audience not found"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Secret") ?? throw new InvalidOperationException("Secret key not found"))),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
