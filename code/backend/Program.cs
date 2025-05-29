using backend.Extensions;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var policyName = builder.Services.AddCorsServices(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });
            builder.Services.AddDatabaseServices(builder.Configuration);
            builder.Services.AddAuthServices(builder.Configuration);
            builder.Services.AddCacheServices(builder.Configuration);

            builder.Services.AddSingleton<ITokenService, TokenService>();

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