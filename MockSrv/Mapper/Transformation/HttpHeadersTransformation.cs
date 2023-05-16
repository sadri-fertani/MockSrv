using AutoMapper;
using System.Text;

namespace MockSrv.Mapper.Transformation
{
    public class HttpHeadersTransformation : IValueConverter<IHeaderDictionary, string>
    {
        public string Convert(IHeaderDictionary headers, ResolutionContext context)
        {
            StringBuilder sb = new StringBuilder("");

            // Filtre
            var keys = headers.Keys.Where(k => !(new[] { "host", "user-agent", "accept", "accept-encoding", "connection", "postman-token" }).Contains(k.ToLower())).ToList();
            keys.Sort();

            // Serialize
            foreach (var key in keys)
                sb.Append($"{key}={headers[key]}&");

            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : string.Empty;
        }
    }
}
