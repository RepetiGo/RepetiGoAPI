using System;
using System.Collections;
using System.Reflection.Metadata;

namespace FlashcardApp.Api
{
    public class ResetCode
    {
        private static Hashtable _codes = new Hashtable();
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public string GenerateCode (string email)
        {
            string code = RandomString(8);
            try
            {
                _codes.Add(email, code);
            }
            catch
            {
                _codes.Remove(email);
                _codes.Add(email, code);
            }
            Countdown(email);
            return code;
        }
        public bool ValidateResetCode (string email, string code)
        {
            if (email == null || code == null)
            {
                return false;
            }
            
            if (code == _codes[email].ToString())
            {
                return true;
            }
            return false;
        }
        private void Countdown(string email)
        {
            Task.Delay(new TimeSpan(0, 15, 0)).ContinueWith(t => {
                try
                {
                    _codes.Remove(email);
                }
                catch
                {

                }
            });
        }
    }
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
            var rateLimitPolicies = builder.Services.AddRateLimitingService();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // Register services
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IDecksService, DecksService>();
            builder.Services.AddScoped<ICardsService, CardsService>();
            builder.Services.AddScoped<IReviewsService, ReviewsService>();
            builder.Services.AddScoped<ISettingsService, SettingsService>();
            builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ResponseTemplate>();
            builder.Services.AddExceptionHandlerService();
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<DataProtectionTokenProviderOptions> (opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));

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
            app.MapControllers().RequireRateLimiting(rateLimitPolicies.Global);

            app.Run();
            
            
        }
    }
}