using MockSrv.Application.Interfaces.DbContextes;
using MockSrv.Domain.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace MockSrv.Api.FuncTests;

[ExcludeFromCodeCoverage]
public static class DbInitializer
{
    private const string FILE_MOCKS_PATH = @"Requests.json";

    public static void SeedData(this IApplicationDbContext context)
    {
        if (!(context?.MockRequests?.Any() ?? false))
        {
            #region Create list of mocks requests
            var mocks = JsonSerializer.Deserialize<List<MockEntity>>(
                File.ReadAllText(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_MOCKS_PATH)
                    )
                )!;
            #endregion
            context?.MockRequests.AddRange(mocks);
        }

        context?.SaveChanges();
    }
}
