namespace MockSrv.Web.Models;

public record HeaderModel
{
    public string Key { get; set; }
    public string ReferenceKey { get; set; }
    public string Value { get; set; }
}
