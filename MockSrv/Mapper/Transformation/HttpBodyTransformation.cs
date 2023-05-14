using AutoMapper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MockSrv.Mapper.Transformation
{
    public class HttpBodyTransformation : IValueConverter<BodyDto, string>
    {
        public string Convert(BodyDto bodySrc, ResolutionContext context)
        {
            if (bodySrc.Body == null)
                return string.Empty;

            string documentContents;

            using (StreamReader readStream = new StreamReader(bodySrc.Body))
            {
                documentContents = readStream.ReadToEndAsync().Result;
            }

            try
            {
                // Clean Format Object
                switch (bodySrc.ContentType)
                {
                    case "text/html":
                    case "text/enriched":
                    case "text/plain":
                        // Corriger le html : On ne sait jamais...
                        if (bodySrc.ContentType.Equals("text/html"))
                        {
                            HtmlDocument htmlContent = new HtmlDocument();
                            htmlContent.LoadHtml(documentContents);

                            documentContents = htmlContent.DocumentNode.OuterHtml;
                        }

                        Regex whitespaceRegex = new Regex(@"(?<=>)\s+?(?=<)");
                        documentContents = whitespaceRegex.Replace(documentContents, string.Empty).Trim();
                        break;
                    case "application/json":
                        if (!string.IsNullOrEmpty(documentContents))
                        {
                            dynamic? obj = JsonConvert.DeserializeObject<dynamic>(documentContents);

                            documentContents = JsonConvert.SerializeObject(obj);
                        }
                        break;
                    case "application/xml":
                    case "text/xml":
                        if (!string.IsNullOrEmpty(documentContents))
                        {
                            XDocument doc = XDocument.Parse(documentContents);

                            documentContents = doc.ToString(SaveOptions.DisableFormatting);
                        }
                        break;
                    default:
                        // Content-Type non officiellement reconnu => R.A.Z
                        documentContents = String.Empty;
                        break;
                }
            }
            catch
            {
                // R.A.Z
                documentContents = string.Empty;
            }

            return documentContents ?? string.Empty;
        }
    }
}
