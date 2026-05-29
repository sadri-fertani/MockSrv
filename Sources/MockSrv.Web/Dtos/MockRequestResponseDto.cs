using System.ComponentModel.DataAnnotations;

namespace MockSrv.Web.Dtos;

public class MockRequestResponseDto
{
    public int Id { get; set; }

    public string? ApiName { get; set; }

    public string? Route { get; set; }

    public required string RequestPath { get; set; }

    public required string RequestMethod { get; set; }

    public string? RequestHeaders { get; set; }

    public string? RequestQueryString { get; set; }

    public string? RequestBody { get; set; }

    public string? ResponseBody { get; set; }

    [Range(100, 599)]
    public required int ResponseStatusCode { get; set; }

    public string? ResponseContentType { get; set; }

    public string? ResponseHeaders { get; set; }
}
