using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace MockSrv.Web.Extensions;

public static class ValidationsExtensions
{
    public static bool IsValidJson(this string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return false;

        // clean
        responseBody = responseBody.Trim();

        if (responseBody.StartsWith('{') && responseBody.EndsWith('}') || //For object
            responseBody.StartsWith('[') && responseBody.EndsWith(']')) //For array
        {
            try
            {
                JToken.Parse(responseBody);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        return false;
    }

    public static bool IsValidXml(this string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            return false;

        try
        {
            using (var xmlReader = XmlReader.Create(new StringReader(responseBody), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Document }))
            {
                while (xmlReader.Read()) { /* Do nothing, just read */ }
            }
            return true;
        }
        catch (XmlException)
        {
            return false;
        }
    }
}
