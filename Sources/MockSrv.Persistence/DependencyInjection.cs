using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockSrv.Application.Interfaces.DbContextes;
using MockSrv.Persistence.DbContexts;
using System.Diagnostics.CodeAnalysis;

namespace MockSrv.Persistence;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<ApplicationDbContext>
            (
                options => 
                {
                    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                }
            )
            .AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }
}
