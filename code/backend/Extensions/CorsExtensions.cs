namespace backend.Extensions
{
    public static class CorsExtensions
    {
        public static string AddCorsServices(this IServiceCollection services, IConfiguration configuration)
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
    }
}
