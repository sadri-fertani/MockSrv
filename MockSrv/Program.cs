using Microsoft.EntityFrameworkCore;
using MockSrv.Models;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddDbContext<MockSrvDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();

app.Run(async context =>
{
    using (var scope = app.Services.CreateScope())
    {
        var contextDb = scope.ServiceProvider.GetService<MockSrvDbContext>();

        if (contextDb == null)
            throw new Exception("DbContext n'a pas été injecter correctement...");

        var mock = await contextDb.MockRequests.FirstOrDefaultAsync(
            m => 
                m.RequestPath.Equals(context.Request.Path.Value) 
                && 
                (
                    m.RequestQueryString.Equals(context.Request.QueryString.Value) 
                    || 
                    m.RequestQueryString == null && string.IsNullOrEmpty(context.Request.QueryString.Value)
                )
            );

        if (mock == null)
            context.Response.StatusCode = 404;
        else
        {
            context.Response.StatusCode = mock.ResponseStatusCode;
            context.Response.ContentType = mock.ResponseContentType ?? String.Empty;
            await context.Response.WriteAsync(mock.ResponseBody ?? String.Empty);
        }
    }
});

app.Run();