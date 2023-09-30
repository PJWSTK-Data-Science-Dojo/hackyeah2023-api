using AspNetCoreRateLimit;

namespace HackYeah_API.Services
{
    public static class RateLimiting
    {
        public static void AddRateLimiting(this IServiceCollection services, IConfigurationSection config)
        {
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(config);
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
