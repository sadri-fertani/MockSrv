using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockSrv.Common.Globals;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;

namespace MockSrv.Api;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });

        services.AddControllers();

        services.AddOutputCache(options =>
        {
            options.AddPolicy(Caches.CACHE_30S, builder => builder.Expire(TimeSpan.FromSeconds(30)));
        });

        return services;
    }
}