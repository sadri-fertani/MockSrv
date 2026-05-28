using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MockSrv.Application;
using MockSrv.Application.DTOs;
using MockSrv.Application.Interfaces.DbContextes;
using MockSrv.Persistence;
using MockSrv.Persistence.DbContexts;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;

namespace MockSrv.Api;

[ExcludeFromCodeCoverage]
public class Program
{
    private const string CACHE_30S = "Expire30";

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
            builder.Host.UseSerilog();

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.SmallestSize;
            });

            builder.Services.AddPersistence(builder.Configuration);

            builder.Services.AddApplication(builder.Configuration);

            builder.Services.AddControllers();

            builder.Services.AddOutputCache(options =>
            {
                options.AddPolicy(CACHE_30S, builder => builder.Expire(TimeSpan.FromSeconds(30)));
            });

            builder.Services
                .AddHealthChecks()
                .AddSqlite(builder.Configuration.GetConnectionString($"DefaultConnection")!, healthQuery: "SELECT 1;", name: "Bilan de santé de la base de donnée")
                .AddDbContextCheck<ApplicationDbContext>("Bilan de santé du DbContext");

            var app = builder.Build();

            // CREATE DB IF NOT EXIST
            using (var srvsScope = app.Services.CreateScope())
            {
                var dbCtx = srvsScope.ServiceProvider.GetService<ApplicationDbContext>();

                if (dbCtx!.Database.IsSqlite())
                    await dbCtx!.Database.EnsureCreatedAsync();
            }

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseOutputCache();

            app.MapControllers();

            app.MapHealthChecks("/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                AllowCachingResponses = false,
                ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            },
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            }).WithMetadata(new AllowAnonymousAttribute());

            app.Map
            (
                "/Mock",
                a => a.Run
                (
                    [OutputCache(PolicyName = CACHE_30S)] 
                    async (contextHttp) =>
                    {
                        using (var scope = app.Services.CreateScope())
                        {
                            var contextDb = scope.ServiceProvider.GetService<IApplicationDbContext>();
                            var mapper = scope.ServiceProvider.GetService<IMapper>();

                            if (contextDb == null)
                                throw new ArgumentException("DbContext n'a pas �t� injecter correctement...");

                            if (mapper == null)
                                throw new ArgumentException("Mapper n'a pas �t� injecter correctement...");

                            var request = mapper.Map<RequestDto>(contextHttp);

                            var mocks = contextDb.MockRequests.Where(
                                m =>
                                    m.RequestPath.ToLower().Equals(request.Path!.ToLower())
                                    &&
                                    m.RequestMethod.ToLower().Equals(request.Method!.ToLower())
                                    &&
                                    (
                                        m.RequestQueryString!.ToLower().Equals(request.QueryString!.ToLower())
                                        ||
                                        m.RequestQueryString == null && string.IsNullOrEmpty(request.QueryString)
                                    )
                                    &&
                                    (
                                        m.RequestBody!.ToLower().Equals(request.Body!.ToLower())
                                        ||
                                        string.IsNullOrEmpty(m.RequestBody)
                                    )
                                );

                            if (!(await mocks.AnyAsync()))
                                contextHttp.Response.StatusCode = 404;
                            else
                            {
                                bool foundOne = false;
                                var rHeaders = request.Headers!.Split('&', StringSplitOptions.RemoveEmptyEntries).ToList();

                                // Check header
                                foreach (var mock in mocks)
                                {
                                    var cHeaders =
                                        mock.RequestHeaders == null ?
                                        [] :
                                        mock.RequestHeaders!.Split('&', StringSplitOptions.RemoveEmptyEntries).ToList();

                                    var match = rHeaders.Intersect(cHeaders).Count().Equals(cHeaders.Count);

                                    if (match)
                                    {
                                        contextHttp.Response.StatusCode = mock.ResponseStatusCode;
                                        contextHttp.Response.ContentType = mock.ResponseContentType ?? string.Empty;

                                        if (!string.IsNullOrEmpty(mock.ResponseHeaders))
                                        {
                                            foreach (var kv in mock.ResponseHeaders.Split('&').Select(m => new KeyValuePair<string, string>(m.Split('=')[0], m.Split('=')[1])))
                                                contextHttp.Response.Headers[kv.Key] = kv.Value;
                                        }

                                        await contextHttp.Response.WriteAsync(mock.ResponseBody ?? string.Empty);
                                        foundOne = true;
                                        break;
                                    }
                                }

                                // There is nothing
                                if (!foundOne)
                                {
                                    contextHttp.Response.StatusCode = 404;
                                }
                            }
                        }
                    }
                )
            );

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