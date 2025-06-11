using System.Text;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace backend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
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

        public static string AddCorsService(this IServiceCollection services, IConfiguration configuration)
        {
            // configure CORS policy
            var origin = configuration.GetValue<string>("Frontend:Url") ?? throw new InvalidOperationException("Frontend URL not found in configuration.");
            var policyName = "AllowSpecificOrigin";

            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                    policy =>
                    {
                        policy.WithOrigins(origin)
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });

            return policyName;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure distributed cache using Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException("Redis connection string not found in configuration.");
            });
            return services;
        }

        public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Entity Framework
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
            });

            return services;
        }

        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            // Configure Identity
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders(); // Add default token providers for password reset, email confirmation, etc.

            return services;
        }

        public static IServiceCollection AddAutoMapperService(this IServiceCollection services)
        {
            // Configure AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }

        public static IServiceCollection AddJsonService(this IServiceCollection services)
        {
            // Configure JSON serialization to ignore null values
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
            return services;
        }
    }
}
