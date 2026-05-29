using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MockSrv.Application.DTOs;
using MockSrv.Application.Interfaces.DbContextes;
using MockSrv.Common.Globals;

namespace MockSrv.Api;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UsePresentation(this WebApplication app)
    {
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
                [OutputCache(PolicyName = Caches.CACHE_30S)]
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

        return app;
    }
}
