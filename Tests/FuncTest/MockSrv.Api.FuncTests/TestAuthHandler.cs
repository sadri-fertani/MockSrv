using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MockSrv.Api.FuncTests;

public class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder, TestClaimsProvider claimsProvider) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        AuthenticateResult authenticateResult;

        if (claimsProvider == null)
            authenticateResult = AuthenticateResult.NoResult();
        else
        {
            var identity = new ClaimsIdentity(claimsProvider.Claims, WebApplicationFactoryExtensions.AUTHENTICATION_TEST_SCHEME);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, WebApplicationFactoryExtensions.AUTHENTICATION_TEST_SCHEME);

            authenticateResult = AuthenticateResult.Success(ticket);
        }

        return Task.FromResult(authenticateResult);
    }
}
