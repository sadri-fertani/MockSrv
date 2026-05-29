using MockSrv.Persistence.DbContexts;

namespace MockSrv.Startup;

public static class PersistenceBuilderExtensions
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
