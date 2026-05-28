using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MockSrv.Application.DTOs;

public class MockRequestResponseDto
{
    public int Id { get; set; }

    public string? ApiName { get; set; }

    public string? Route { get; set; }

    [Required]
    public string? RequestPath { get; set; }

    [Required]
    public string? RequestMethod { get; set; }

    public string? RequestHeaders { get; set; }

    public string? RequestQueryString { get; set; }

    public string? RequestBody { get; set; }

    public string? ResponseBody { get; set; }

    [Required]
    [Range(100, 599)]
    public int ResponseStatusCode { get; set; }

    public string? ResponseContentType { get; set; }

    public string? ResponseHeaders { get; set; }
}