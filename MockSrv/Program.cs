using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MockSrv.DTOs;
using MockSrv.Mapper;
using MockSrv.Models;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddAutoMapper(typeof(ApplicationProfile));

builder
    .Services.AddDbContext<MockSrvDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();

app.Run(async contextHttp =>
{
    using (var scope = app.Services.CreateScope())
    {
        var contextDb = scope.ServiceProvider.GetService<MockSrvDbContext>();
        var mapper = scope.ServiceProvider.GetService<IMapper>();

        if (contextDb == null)
            throw new Exception("DbContext n'a pas été injecter correctement...");

        if (mapper == null)
            throw new Exception("Mapper n'a pas été injecter correctement...");

        var request = mapper.Map<RequestDto>(contextHttp);

        var mock = await contextDb.MockRequest.FirstOrDefaultAsync(
            m =>
                m.RequestPath.Equals(request.Path)
                &&
                m.RequestMethod.Equals(request.Method)
                &&
                (
                    m.RequestHeaders.Equals(request.Headers)
                    ||
                    m.RequestHeaders == null && string.IsNullOrEmpty(request.Headers)
                )
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
            contextHttp.Response.StatusCode = 404;
        else
        {
            contextHttp.Response.StatusCode = mock.ResponseStatusCode;
            contextHttp.Response.ContentType = mock.ResponseContentType ?? string.Empty;

            if(!string.IsNullOrEmpty(mock.ResponseHeaders))
            {
                foreach(var kv in mock.ResponseHeaders.Split('&').Select(m => new KeyValuePair<string, string>(m.Split('=')[0], m.Split('=')[1])))
                    contextHttp.Response.Headers.Add(kv.Key, kv.Value);
            }
            
            await contextHttp.Response.WriteAsync(mock.ResponseBody ?? string.Empty);
        }
    }
});

app.Run();