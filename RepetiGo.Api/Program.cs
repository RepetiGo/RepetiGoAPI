namespace RepetiGo.Api
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // Configure services
            builder.Services.AddOpenApiService()
                .AddConfigurationService(builder.Configuration)
                .AddCorsService("AllowedHosts")
                .AddDatabaseService()
                .AddIdentityService()
                .AddAuthenticationService()
                .AddCacheService()
                .AddAutoMapperService()
                .AddJsonService()
                .AddDataProtectionTokenProviderOptionsService()
                .AddFormOptionsService()
                .AddExceptionHandlerService()
                .AddHttpContextAccessor()
                .AddHttpLogging(o => { })
                .AddResiliencePipelineService()
                .AddRateLimitingService();

            // Register services
            builder.Services.AddScoped<IUsersService, UsersService>()
                .AddScoped<IDecksService, DecksService>()
                .AddScoped<ICardsService, CardsService>()
                .AddScoped<ISettingsService, SettingsService>()
                .AddScoped<IEmailSenderService, EmailSenderService>()
                .AddScoped<IUploadsService, UploadsService>()
                .AddScoped<IAiGeneratorService, AiGeneratorService>()
                .AddScoped<IReviewsService, ReviewsService>()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<DatabaseSeeders>();

            var app = builder.Build();

            app.UseHttpLogging();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "RepetiGo API");
                });
                using var scope = app.Services.CreateScope();
                var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeders>();
                await seeder.SeedDatabaseAsync();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseCors("AllowedHosts");
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers().RequireRateLimiting("global");

            app.Run();
        }
    }
}