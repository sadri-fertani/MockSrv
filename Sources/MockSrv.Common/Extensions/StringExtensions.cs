using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MockSrv.Common.Extensions;

public static class StringExtensions
{
    public static string? Clean(this string str)
    {
        if (str != null && str.Length > 0)
        {
            str = str.Trim() switch
            {
                string a when a.StartsWith('[') => CleanJson(str.Trim()),
                string a when a.StartsWith('{') => CleanJson(str.Trim()),
                string a when a.StartsWith("<!doctype html>", StringComparison.OrdinalIgnoreCase) => CleanHtml(str.Trim()),
                string a when a.StartsWith("<html", StringComparison.OrdinalIgnoreCase) => CleanHtml(str.Trim()),
                string a when a.StartsWith('<') => CleanXml(str.Trim()),
                _ => str.Trim()
            };
        }

        return str;
    }

    private static string CleanJson(string documentContents)
    {
        try
        {
            if (!string.IsNullOrEmpty(documentContents))
            {
                dynamic? obj = JsonConvert.DeserializeObject<dynamic>(documentContents);

                // Clean dynamic object
                obj = JsonExtensions.CleanDynamic(obj);

                documentContents = JsonConvert.SerializeObject(obj);
            }

            return documentContents;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string CleanXml(string documentContents)
    {
        try
        {
            if (!string.IsNullOrEmpty(documentContents))
            {
                XDocument doc = XDocument.Parse(documentContents);

                documentContents = doc.ToString(SaveOptions.DisableFormatting);
            }

            return documentContents;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string CleanHtml(string documentContents)
    {
        try
        {
            // Corriger le html : On ne sait jamais...
            HtmlDocument htmlContent = new HtmlDocument();
            htmlContent.LoadHtml(documentContents);

            documentContents = htmlContent.DocumentNode.OuterHtml;

            Regex whitespaceRegex = new Regex(@"(?<=>)\s+?(?=<)");
            documentContents = whitespaceRegex.Replace(documentContents, string.Empty).Trim();

            return documentContents;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string RemoveNewLines(string documentContents)
    {
        return string.Concat(documentContents.Split('\n'));
    }
}

