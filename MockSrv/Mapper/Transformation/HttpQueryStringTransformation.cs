using AutoMapper;
using System.Text;

namespace MockSrv.Mapper.Transformation
{
    public class HttpQueryStringTransformation : IValueConverter<string, string>
    {
        public string Convert(string query, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(query))
                return string.Empty;

            // Split
            var cleanKeysValues = query
                .Split('?')
                .Where(m => m.Contains('='))
                .SelectMany(pr => pr.Split('&'))
                .Select(m => new KeyValuePair<string, string>(m.Split('=')[0], m.Split('=')[1]))
                .ToList()
                .OrderBy(x => x.Key)
                .DistinctBy(x => x.Key);

            StringBuilder sb = new StringBuilder("?");

            // Join
            foreach (var kv in cleanKeysValues)
                sb.Append($"{kv.Key}={kv.Value}&");

            return sb.Remove(sb.Length - 1, 1).ToString();
        }
    }
}
