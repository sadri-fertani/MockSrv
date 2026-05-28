using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockSrv.Domain.Entities;

[Table("MockRequest")]
public partial class MockEntity
{
    [Key]
    public int Id { get; set; }

    public string? HashKey { get; set; }

    public required string RequestPath { get; set; }

    public required string RequestMethod { get; set; }

    public string? RequestHeaders { get; set; }

    public string? RequestQueryString { get; set; }

    public string? RequestBody { get; set; }

    public string? ResponseBody { get; set; }

    public required int ResponseStatusCode { get; set; }

    public string? ResponseContentType { get; set; }

    public string? ResponseHeaders { get; set; }
}

