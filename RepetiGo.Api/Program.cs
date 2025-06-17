namespace RepetiGo.Api
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var corsPolicyName = "AllowedHosts";

            // Configure services
            builder.Services.AddControllers();
            var (AiGeneration, Global) = builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>())
                .AddConfigurationService(builder.Configuration)
                .AddCorsService(corsPolicyName)
                .AddDatabaseService()
                .AddIdentityService()
                .AddAuthenticationService()
                .AddCacheService()
                .AddAutoMapperService()
                .AddJsonService()
                .AddExceptionHandler<GlobalExceptionHandler>()
                .Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2))
                .ConfigureFormOptions()
                .AddExceptionHandlerService()
                .AddHttpContextAccessor()
                .AddHttpLogging(o => { })
                .AddRateLimitingService();

            // Register services
            builder.Services.AddScoped<IUsersService, UsersService>()
                .AddScoped<IDecksService, DecksService>()
                .AddScoped<ICardsService, CardsService>()
                .AddScoped<ISettingsService, SettingsService>()
                .AddScoped<IEmailSenderService, EmailSenderService>()
                .AddScoped<IUploadsService, UploadsService>()
                .AddScoped<IAiGeneratorService, AiGeneratorService>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<ResponseTemplate>()
                .AddScoped<ResetCode>();

            var app = builder.Build();

            app.UseHttpLogging();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "RepetiGo API");
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseCors(corsPolicyName);
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers().RequireRateLimiting(Global);

            app.Run();
        }
    }
}