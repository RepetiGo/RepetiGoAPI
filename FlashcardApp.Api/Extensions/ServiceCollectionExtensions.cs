using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace FlashcardApp.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                // Configure the way we validate the received token
                options.SaveToken = false; // Do not save the token in HttpContext
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
                        policy.AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials()
                              //.WithOrigins(origin)
                              .SetIsOriginAllowed(origin => true); // Allow any origin for development purposes
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
                options.Password.RequiredUniqueChars = 4;
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

        public static (string AiGeneration, string Global) AddRateLimitingService(this IServiceCollection services)
        {
            // Configure rate limiting
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("ai-generation", options =>
                {
                    options.PermitLimit = 20;
                    options.Window = TimeSpan.FromHours(1);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 2;
                });

                options.AddSlidingWindowLimiter("global", options =>
                {
                    options.PermitLimit = 100;
                    options.Window = TimeSpan.FromMinutes(1);
                    options.SegmentsPerWindow = 5;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 5;
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsJsonAsync(
                        "Too many requests. Please try again later.",
                        cancellationToken: token
                    );
                };
            });

            (string AiGeneration, string Global) rateLimitPolicies = (AiGeneration: "ai-generation", Global: "global");
            return rateLimitPolicies;
        }

        public static IServiceCollection AddExceptionHandlerService(this IServiceCollection services)
        {
            // Register a global exception handler
            services.AddExceptionHandler<GlobalExceptionHandler>();
            return services;
        }
    }
}
