using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MockSrv.Persistence.DbContexts;

namespace MockSrv.Persistence;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UsePersistence(this WebApplication app)
    {
        // CREATE DB IF NOT EXIST
        using (var srvsScope = app.Services.CreateScope())
        {
            var dbCtx = srvsScope.ServiceProvider.GetService<ApplicationDbContext>();
            dbCtx!.Database.EnsureCreatedAsync().Wait();
        }

        return app;
    }
}
