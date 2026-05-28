using AutoMapper;
using MockSrv.Domain.Extensions;

namespace MockSrv.Application.Mapper.Transformations;

public class HttpBodyTransformation : IValueConverter<Stream?, string>
{
    public string Convert(Stream? bodySrc, ResolutionContext context)
    {
        if (bodySrc == null)
            return string.Empty;

        string? documentContents;

        using (StreamReader readStream = new(bodySrc))
        {
            documentContents = readStream.ReadToEndAsync().Result;
        }

        try
        {
            documentContents = documentContents?.Clean();
        }
        catch
        {
            // R.A.Z
            documentContents = string.Empty;
        }

        return documentContents ?? string.Empty;
    }
}
