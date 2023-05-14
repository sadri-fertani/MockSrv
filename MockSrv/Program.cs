using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MockSrv.DTOs;
using MockSrv.Mapper;
using MockSrv.Models;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddAutoMapper(typeof(ApplicationProfile));

builder
    .Services.AddDbContext<MockSrvDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();

app.Run(async context =>
{
    using (var scope = app.Services.CreateScope())
    {
        var contextDb = scope.ServiceProvider.GetService<MockSrvDbContext>();
        var mapper = scope.ServiceProvider.GetService<IMapper>();

        if (contextDb == null)
            throw new Exception("DbContext n'a pas été injecter correctement...");

        if (mapper == null)
            throw new Exception("Mapper n'a pas été injecter correctement...");

        var request = mapper.Map<RequestDto>(context);

        var mock = await contextDb.MockRequest.FirstOrDefaultAsync(
            m =>
                m.RequestPath.Equals(request.Path)
                &&
                m.RequestMethod.Equals(request.Method)
                &&
                (
                    m.RequestQueryString.Equals(request.QueryString)
                    ||
                    m.RequestQueryString == null && string.IsNullOrEmpty(request.QueryString)
                )
                &&
                (
                    m.RequestBody.Equals(request.Body)
                    ||
                    m.RequestBody == null && string.IsNullOrEmpty(request.Body)
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