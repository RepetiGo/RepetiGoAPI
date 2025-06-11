using FlashcardApp.Api.Repositories;
using FlashcardApp.Api.Services;

namespace FlashcardApp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var policyName = builder.Services.AddCorsService(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });
            builder.Services.AddDatabaseService(builder.Configuration);
            builder.Services.AddIdentityService();
            builder.Services.AddAuthenticationService(builder.Configuration);
            builder.Services.AddCacheService(builder.Configuration);
            builder.Services.AddAutoMapperService();
            builder.Services.AddJsonService();

            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IDecksService, DecksService>();
            builder.Services.AddScoped<ICardsService, CardsService>();
            builder.Services.AddScoped<IReviewsService, ReviewsService>();
            builder.Services.AddScoped<ISettingsService, SettingsService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Flashcard Application");
                });
            }

            app.UseHttpsRedirection();
            app.UseCors(policyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}