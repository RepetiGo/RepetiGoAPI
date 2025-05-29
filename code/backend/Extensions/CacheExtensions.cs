namespace backend.Extensions
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure distributed cache using Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException("Redis connection string not found in configuration.");
            });
            return services;
        }
    }
}