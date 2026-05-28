using MockSrv.Application.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace MockSrv.Api.FuncTests;

[TestCaseOrderer("MockSrv.Api.FuncTests.PriorityOrderer", "MockSrv.Api.FuncTests")]
public class UnitTestEndPoints : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private const string PREFIX_ROUTE_Mock = "/Mock";

    private const string PREFIX_ROUTE_Admin = "/Admin";

    public UnitTestEndPoints(CustomWebApplicationFactory<Program> factory)
    {
        var claimsProvider = TestClaimsProvider.WithBasicUserClaims();
        _client = factory.CreateClientWithTestAuth(claimsProvider);
    }

    #region Route : /Mock
    [Fact, TestPriority(1)]
    public async Task Test_RouteMock_NotFound()
    {
        var r = await _client.GetAsync($"{PREFIX_ROUTE_Mock}/");

        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    [Fact, TestPriority(2)]
    public async Task Test_RouteMock_Request_Get_Response_Text()
    {
        var r = await _client.GetAsync($"{PREFIX_ROUTE_Mock}/a/testGet");

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal(MediaTypeHeaderValue.Parse("text/enriched"), r.Content.Headers.ContentType);
        Assert.Equal("Default", body);
    }

    [Fact, TestPriority(3)]
    public async Task Test_RouteMock_Request_Post_Response_Json()
    {
        var r = await _client.PostAsync
        (
            $"{PREFIX_ROUTE_Mock}/b/testPost",
            new StringContent
            (
                JsonConvert.SerializeObject(new { id = 0, name = "sadri" }),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            )
        );

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        Assert.Equal(MediaTypeHeaderValue.Parse("application/json"), r.Content.Headers.ContentType);
        Assert.Equal(JsonConvert.SerializeObject(new { id = 1, name = "sadri" }), body);
    }

    [Fact, TestPriority(4)]
    public async Task Test_RouteMock_Request_Put_Response_Json()
    {
        var r = await _client.PutAsync
        (
            $"{PREFIX_ROUTE_Mock}/c/testPut",
            new StringContent
            (
                JsonConvert.SerializeObject(new { id = 2, name = "sadri new" }),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            )
        );

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        Assert.Equal(MediaTypeHeaderValue.Parse("application/json"), r.Content.Headers.ContentType);
        Assert.Equal(JsonConvert.SerializeObject(new { id = 2, name = "sadri new" }), body);
    }

    [Fact, TestPriority(5)]
    public async Task Test_RouteMock_Request_Delete_Response_NoContent()
    {
        var r = await _client.DeleteAsync
        (
            $"{PREFIX_ROUTE_Mock}/c/testDelete/2"
        );
        
        await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    [Fact, TestPriority(6)]
    public async Task Test_RouteMock_Request_Headers_Response_OK()
    {
        _client.DefaultRequestHeaders.Add("k1", "v1");
        _client.DefaultRequestHeaders.Add("k2", "v2");

        var r = await _client.GetAsync($"{PREFIX_ROUTE_Mock}/a/testHeaders");

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal(MediaTypeHeaderValue.Parse("text/enriched"), r.Content.Headers.ContentType);
        Assert.Equal("Default", body);
    }

    [Fact, TestPriority(7)]
    public async Task Test_RouteMock_Request_Headers_Response_NotFound()
    {
        _client.DefaultRequestHeaders.Add("k1", "v1");

        var r = await _client.GetAsync($"{PREFIX_ROUTE_Mock}/a/testHeaders");

        await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    [Fact, TestPriority(8)]
    public async Task Test_RouteMock_Request_QueryString_Response_OK()
    {
        var r = await _client.GetAsync($"{PREFIX_ROUTE_Mock}/a/testQueryString?k1=v1&k2=v2&k3=v3");

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal(MediaTypeHeaderValue.Parse("text/enriched"), r.Content.Headers.ContentType);
        Assert.Equal("Default", body);
    }

    [Fact, TestPriority(9)]
    public async Task Test_RouteMock_Request_QueryString_Response_NotFound()
    {
        var r = await _client.GetAsync($"{PREFIX_ROUTE_Mock}/a/testQueryString?k1=v1");

        await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    #endregion

    #region Route : /Admin
    [Fact, TestPriority(21)]
    public async Task Test_RouteAdmin_Request_GetAll()
    {
        var r = await _client.GetAsync($"{PREFIX_ROUTE_Admin}");

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal("application/json", r.Content.Headers.ContentType!.MediaType);

        var mocks = JsonConvert.DeserializeObject<List<MockRequestResponseDto>>(body);

        Assert.NotNull(mocks);
    }

    [Fact, TestPriority(22)]
    public async Task Test_RouteAdmin_Request_GetOne_NotFound()
    {
        var r = await _client.GetAsync($"{PREFIX_ROUTE_Admin}/97691");

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    [Fact, TestPriority(23)]
    public async Task Test_RouteAdmin_Request_GetOne()
    {
        var r = await _client.GetAsync($"{PREFIX_ROUTE_Admin}/1");

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal("application/json", r.Content.Headers.ContentType!.MediaType);

        var mock = JsonConvert.DeserializeObject<MockRequestResponseDto>(body);
        Assert.NotNull(mock);

        Assert.Equal(1, mock.Id);
    }

    [Fact, TestPriority(24)]
    public async Task Test_RouteAdmin_Request_Post()
    {
        var r = await _client.PostAsync
        (
            $"{PREFIX_ROUTE_Admin}",
            new StringContent
            (
                JsonConvert.SerializeObject
                (
                    new MockRequestResponseDto
                    {
                        Id = 0,
                        ApiName = "head",
                        Route = "head1",
                        RequestPath = "/b/testPost",
                        RequestMethod = "POST",
                        RequestHeaders = "",
                        RequestBody = "{\"id\":0,\"name\":\"sadri\"}",
                        ResponseStatusCode = 200,
                        ResponseContentType = "text/enriched",
                        ResponseHeaders = "sf-key-04=1&sf-key-03=2&qw-ui-oP=ok"
                    }
                ),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            )
        );

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        Assert.Equal("application/json", r.Content.Headers.ContentType!.MediaType);

        var mock = JsonConvert.DeserializeObject<MockRequestResponseDto>(body);
        Assert.NotNull(mock);

        Assert.NotEqual(0, mock.Id);
    }

    [Fact, TestPriority(25)]
    public async Task Test_RouteAdmin_Request_Put_NotFound()
    {
        var r = await _client.PutAsync
        (
            $"{PREFIX_ROUTE_Admin}",
            new StringContent
            (
                JsonConvert.SerializeObject
                (
                    new MockRequestResponseDto
                    {
                        Id = 97691,
                        ApiName = "head",
                        Route = "head1",
                        RequestPath = "/head/head1",
                        RequestMethod = "Get",
                        RequestHeaders = "sf-key-01=1&sf-key-02=2",
                        ResponseBody = "ok test headers 1",
                        ResponseStatusCode = 200,
                        ResponseContentType = "text/enriched",
                        ResponseHeaders = "sf-key-04=1&sf-key-03=2&qw-ui-oP=ok"
                    }
                ),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            )
        );

        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    [Fact, TestPriority(26)]
    public async Task Test_RouteAdmin_Request_Put()
    {
        var r = await _client.PutAsync
        (
            $"{PREFIX_ROUTE_Admin}",
            new StringContent
            (
                JsonConvert.SerializeObject
                (
                    new MockRequestResponseDto
                    {
                        Id = 1,
                        ApiName = "head",
                        Route = "head1",
                        RequestPath = "/head/head1",
                        RequestMethod = "Get",
                        RequestHeaders = "sf-key-01=1&sf-key-02=2",
                        ResponseBody = "ok test headers 1",
                        ResponseStatusCode = 200,
                        ResponseContentType = "text/enriched",
                        ResponseHeaders = "sf-key-04=1&sf-key-03=2&qw-ui-oP=ok"
                    }
                ),
                Encoding.UTF8,
                new MediaTypeHeaderValue("application/json")
            )
        );

        var body = await r.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal("application/json", r.Content.Headers.ContentType!.MediaType);

        var mock = JsonConvert.DeserializeObject<MockRequestResponseDto>(body);
        Assert.NotNull(mock);

        Assert.Equal(1, mock.Id);
    }

    [Fact, TestPriority(27)]
    public async Task Test_RouteAdmin_Request_Delete()
    {
        var r = await _client.DeleteAsync
        (
            $"{PREFIX_ROUTE_Admin}/4"
        );

        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    //[Fact, TestPriority(28)]
    //public async Task Test_RouteAdmin_Request_Delete_Forbidden()
    //{
    //    var r = await _client.DeleteAsync
    //    (
    //        $"{PREFIX_ROUTE_Admin}/6"
    //    );

    //    Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
    //}

    //[Fact, TestPriority(27)]
    //public async Task Test_RouteAdmin_Request_Put_Forbidden()
    //{
    //    var r = await _client.PutAsync
    //    (
    //        $"{PREFIX_ROUTE_Admin}",
    //        new StringContent
    //        (
    //            JsonConvert.SerializeObject
    //            (
    //                new MockRequestResponseDto
    //                {
    //                    Id = 6,
    //                    ApiName = "head",
    //                    Route = "head1",
    //                    RequestPath = "/head/head1",
    //                    RequestMethod = "Get",
    //                    RequestHeaders = "sf-key-01=1&sf-key-02=2",
    //                    ResponseBody = "ok test headers 1",
    //                    ResponseStatusCode = 200,
    //                    ResponseContentType = "text/enriched",
    //                    ResponseHeaders = "sf-key-04=1&sf-key-03=2&qw-ui-oP=ok"
    //                }
    //            ),
    //            Encoding.UTF8,
    //            new MediaTypeHeaderValue("application/json")
    //        )
    //    );

    //    Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
    //}
    #endregion
}