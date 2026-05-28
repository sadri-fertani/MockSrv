using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace MockSrv.Api.FuncTests;

public static class WebApplicationFactoryExtensions
{
    public const string AUTHENTICATION_TEST_SCHEME = "AuthenticationTestScheme";
    public static WebApplicationFactory<T> WithAuthentication<T>(this WebApplicationFactory<T> factory, TestClaimsProvider claimsProvider) where T : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(AUTHENTICATION_TEST_SCHEME)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(AUTHENTICATION_TEST_SCHEME, op => { });

                services.AddScoped<TestClaimsProvider>(_ => claimsProvider);
            });
        });
    }

    public static HttpClient CreateClientWithTestAuth<T>(this WebApplicationFactory<T> factory, TestClaimsProvider claimsProvider) where T : class
    {
        var client = factory.WithAuthentication(claimsProvider).CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AUTHENTICATION_TEST_SCHEME);

        return client;
    }
}
