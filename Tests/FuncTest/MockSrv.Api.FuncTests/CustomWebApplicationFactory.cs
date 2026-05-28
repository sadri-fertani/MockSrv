using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MockSrv.Application.Interfaces.DbContextes;
using MockSrv.Persistence.DbContexts;

namespace MockSrv.Api.FuncTests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    /// <summary>
    /// Overriding CreateHost to avoid creating a separate ServiceProvider per this thread:
    /// https://github.com/dotnet-architecture/eShopOnWeb/issues/465
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Configure fake auth provider
        builder.ConfigureServices(srvCollection =>
        {
            // Remove all schema auth
            srvCollection.Configure<AuthenticationOptions>(o =>
            {
                if (o.Schemes is List<AuthenticationSchemeBuilder> schemes)
                {
                    schemes.RemoveAll(s => s.Name == NegotiateDefaults.AuthenticationScheme);
                    o.SchemeMap.Remove(NegotiateDefaults.AuthenticationScheme);
                }
            });

            srvCollection.AddAuthorizationBuilder();
        });

        var host = builder.Build();
        host.Start();

        // Get service provider.
        var serviceProvider = host.Services;

        // Create a scope to obtain a reference to the database
        // context (AppDbContext).
        using (var scope = serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            
            var db = scopedServices
                .GetRequiredService<IApplicationDbContext>();

            var logger = scopedServices
                .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

            // Ensure the database is created.
            db.Database.EnsureCreated();

            try
            {
                // Seed the database
                db.SeedData();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database with test messages. Error: {Message}", ex.Message);
            }
        }

        return host;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // This should be set for each individual test run
                string inMemoryCollectionName = Guid.NewGuid().ToString();

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(inMemoryCollectionName);
                });
            });
    }
}
