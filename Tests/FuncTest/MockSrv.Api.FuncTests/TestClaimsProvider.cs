using System.Security.Claims;

namespace MockSrv.Api.FuncTests;

public class TestClaimsProvider
{
    public IList<Claim> Claims { get; }

    public TestClaimsProvider(IList<Claim> claims)
    {
        Claims = claims;
    }

    public TestClaimsProvider()
    {
        Claims = new List<Claim>();
    }

    public static TestClaimsProvider? WithAnonymousUserClaims()
    {
        return null;
    }

    public static TestClaimsProvider WithBasicUserClaims()
    {
        var provider = new TestClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, "Test.User.Fake"));
        provider.Claims.Add(new Claim(ClaimTypes.Name, "FAKE\\SFE093"));
        provider.Claims.Add(new Claim(ClaimTypes.Email, "test.user@api.basic"));

        return provider;
    }
}
