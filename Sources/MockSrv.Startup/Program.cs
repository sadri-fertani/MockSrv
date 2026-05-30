using MockSrv.Api;
using MockSrv.Application;
using MockSrv.Common.Logging;
using MockSrv.Persistence;
using MockSrv.Persistence.DbContexts;
using Serilog;

namespace MockSrv.Startup;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            // Configuration Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("Logs/api-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Démarrage de l'application API MockSrv");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host
                .UseSerilog();

            builder.Services
                .AddSanitizedLogger()
                .AddPersistence(builder.Configuration)
                .AddApplication(builder.Configuration)
                .AddPresentation(builder.Configuration)
                .AddHealthChecks()
                .AddSqlite(builder.Configuration.GetConnectionString($"DefaultConnection")!, healthQuery: "SELECT 1;", name: "Bilan de santé de la base de donnée")
                .AddDbContextCheck<ApplicationDbContext>("Bilan de santé du DbContext");

            var app = builder.Build();

            app
                .UsePersistence()
                .UsePresentation();

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "L'application s'est arrêtée de manière inattendue");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
