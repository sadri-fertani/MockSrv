using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace MockSrv.Application.Mapper.Transformations;

public class HttpHeadersTransformation : IValueConverter<IHeaderDictionary, string>
{
    public string Convert(IHeaderDictionary headers, ResolutionContext context)
    {
        StringBuilder sb = new("");

        var keys = headers.Keys
            .Where(k => !(new[] { "Authorization" })
            .Contains(k.ToLower()))
            .ToList();

        keys.Sort();

        // Custom Serialize like querystring
        foreach (var key in keys)
            sb.Append($"{key}={headers[key]}&");

        return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : string.Empty;
    }
}