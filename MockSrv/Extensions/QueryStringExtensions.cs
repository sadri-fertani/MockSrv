using System.Text;

public static class QueryStringExtensions
{
    public static string Clean(this string? query)
    {
        if(string.IsNullOrEmpty(query))
            return String.Empty;

        // Split
        var cleanKeysValues = query
            .Split('?')
            .Where(m=>m.Contains('='))
            .SelectMany(pr => pr.Split('&'))
            .Select(m=> new KeyValuePair<string,string>(m.Split('=')[0], m.Split('=')[1]))
            .ToList()
            .OrderBy(x => x.Key)
            .DistinctBy(x => x.Key);

        StringBuilder sb = new StringBuilder("?");

        // Join
        foreach(var kv in cleanKeysValues)
            sb.Append($"{kv.Key}={kv.Value}&");

        return sb.Remove(sb.Length - 1, 1).ToString();
    }
}