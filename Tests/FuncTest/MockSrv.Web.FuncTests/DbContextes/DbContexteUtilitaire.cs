using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Common;
using MockSrv.Domain.Entities;
using MockSrv.Persistence.DbContexts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MockSrv.Web.FuncTests.DbContextes;

[ExcludeFromCodeCoverage]
public static class DbContexteUtilitaire
{
    /// <summary>
    /// Create DbContextOptions
    /// </summary>
    /// <returns>new DbContextOptions</returns>
    public static DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
    {
        // Create a fresh service provider, and therefore a fresh InMemory database instance.
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        // Create a new options instance telling the context to use an InMemory database and the new service provider.
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        builder
            .UseInMemoryDatabase(databaseName: "CNESST_Db_InMemory")
            .UseInternalServiceProvider(serviceProvider);

        return builder.Options;
    }

    public static void Populate(this ApplicationDbContext context)
    {
        context.MockRequests.AddRange(
            new List<MockEntity>()
            {
                    new MockEntity
                    {
                        RequestPath= "/a/test",
                        RequestMethod= "Get",
                        ResponseBody= "Default",
                        ResponseStatusCode= 200,
                        ResponseContentType= "text/enriched",
                        HashKey=string.Concat("/a/test","Get")
                    }
            });

        context.SaveChanges();
    }

    public static ApplicationDbContext Get()
    {
        var context = new ApplicationDbContext(CreateNewContextOptions());

        // Insert seed data into the database using one instance of the context
        context.Populate();

        return context;
    }
}


