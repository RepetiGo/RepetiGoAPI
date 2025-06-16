using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using RepetiGo.Api.ConfigModels;
using RepetiGo.Api.Data;
using RepetiGo.Api.Helpers;
using RepetiGo.Api.Models;

namespace RepetiGo.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationService(this IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            var jwtConfig = serviceProvider.GetRequiredService<IOptions<JwtConfig>>().Value;

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
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        public static IServiceCollection AddCorsService(this IServiceCollection services, string policyName)
        {
            // configure CORS policy
            using var serviceProvider = services.BuildServiceProvider();
            var clientConfig = serviceProvider.GetRequiredService<IOptions<ClientConfig>>().Value;

            if (string.IsNullOrWhiteSpace(clientConfig.WebAppUrl) || clientConfig.WebAppUrl == "*")
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(policyName,
                        policy =>
                        {
                            policy.AllowAnyHeader() // Allow any header in the request
                                  .AllowAnyMethod() // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                                  .AllowCredentials() // Allow credentials for cookies, authorization headers, or TLS client certificates
                                  .SetIsOriginAllowed(origin => true); // Allow any origin for development purposes
                        });
                });
            }
            else
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(policyName,
                        policy =>
                        {
                            policy.AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials()
                                  .WithOrigins(clientConfig.WebAppUrl);
                        });
                });
            }

            return services;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services)
        {
            // Configure distributed cache using Redis
            using var serviceProvider = services.BuildServiceProvider();
            var connectionStringConfig = serviceProvider.GetRequiredService<IOptions<ConnectionStringConfig>>().Value;

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionStringConfig.RedisConnection;
            });
            return services;
        }

        public static IServiceCollection AddDatabaseService(this IServiceCollection services)
        {
            // Configure Entity Framework
            using var serviceProvider = services.BuildServiceProvider();
            var connectionStringConfig = serviceProvider.GetRequiredService<IOptions<ConnectionStringConfig>>().Value;

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionStringConfig.DefaultConnection);
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

        public static IServiceCollection AddConfigurationService(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure application settings from configuration
            services.AddOptions<ConnectionStringConfig>()
                .Bind(configuration.GetSection(ConnectionStringConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<JwtConfig>()
                .Bind(configuration.GetSection(JwtConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<ClientConfig>()
                .Bind(configuration.GetSection(ClientConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<EmailSettingsConfig>()
                .Bind(configuration.GetSection(EmailSettingsConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<CloudinaryConfig>()
                .Bind(configuration.GetSection(CloudinaryConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<OAuthConfig>()
                .Bind(configuration.GetSection(OAuthConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<GoogleGeminiConfig>()
                .Bind(configuration.GetSection(GoogleGeminiConfig.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }

        public static IServiceCollection ConfigureFormOptions(this IServiceCollection services)
        {
            // Configure form options to allow larger file uploads
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 2048; // Maximum number of form values
            });

            return services;
        }
    }
}
