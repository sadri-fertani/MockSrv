using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MockSrv.Common.Logging;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddSanitizedLogger(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ISanitizedLogger<>), typeof(SanitizedLogger<>));
        return services;
    }
}
