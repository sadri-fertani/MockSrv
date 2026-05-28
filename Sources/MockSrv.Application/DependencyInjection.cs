using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockSrv.Application.Interfaces.Services;
using MockSrv.Application.Mapper;
using MockSrv.Application.Services;
using System.Diagnostics.CodeAnalysis;

namespace MockSrv.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(cfg => { cfg.AddProfile<ApplicationProfile>(); });

        services.AddScoped<IMockRequestResponseService, MockRequestResponseService>();

        return services;
    }
}
