using FlashcardApp.Api.Interfaces;

namespace FlashcardApp.Api
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            builder.Services.AddControllers();
            builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
            var policyName = builder.Services.AddCorsService(builder.Configuration);
            var (AiGeneration, Global) = builder.Services.AddRateLimitingService();
            builder.Services.AddDatabaseService(builder.Configuration);
            builder.Services.AddIdentityService();
            builder.Services.AddAuthenticationService(builder.Configuration);
            builder.Services.AddCacheService(builder.Configuration);
            builder.Services.AddAutoMapperService();
            builder.Services.AddJsonService();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddConfigurationService(builder.Configuration);
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));
            builder.Services.ConfigureFormOptions();

            // Register services
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IDecksService, DecksService>();
            builder.Services.AddScoped<ICardsService, CardsService>();
            builder.Services.AddScoped<IReviewsService, ReviewsService>();
            builder.Services.AddScoped<ISettingsService, SettingsService>();
            builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
            builder.Services.AddScoped<IUploadsService, UploadsService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ResponseTemplate>();
            builder.Services.AddScoped<ResetCode>();
            builder.Services.AddExceptionHandlerService();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Flashcard Application");
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseCors(policyName);
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers().RequireRateLimiting(Global);

            app.Run();


        }
    }
}