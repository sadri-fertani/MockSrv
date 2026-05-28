using AutoMapper;
using Microsoft.AspNetCore.Http;
using MockSrv.Application.DTOs;
using MockSrv.Application.Mapper.Transformations;
using MockSrv.Domain.Entities;

namespace MockSrv.Application.Mapper;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<MockEntity, MockRequestResponseDto>()
            .ForMember(dest => dest.ApiName, opt => opt.MapFrom(src => GetApiNameFromPath(src.RequestPath)))
            .ForMember(dest => dest.Route, opt => opt.MapFrom(src => GetRouteFromPath(src.RequestPath)));

        CreateMap<MockRequestResponseDto, MockEntity>();

        CreateMap<HttpContext, RequestDto>()
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Request.Path))
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Request.Method))
            .ForMember
            (
                dest => dest.Headers,
                opt => opt.ConvertUsing
                (
                    new HttpHeadersTransformation(),
                    src => src.Request.Headers
                )
            )
            .ForMember
            (
                dest => dest.QueryString,
                opt => opt.ConvertUsing
                (
                    new HttpQueryStringTransformation(),
                    src => src.Request.QueryString.Value ?? string.Empty
                )
            )
            .ForMember
            (
                dest => dest.Body,
                opt => opt.ConvertUsing
                (
                    new HttpBodyTransformation(),
                    src => src.Request.Body
                )
            );
    }

    private static string GetApiNameFromPath(string path)
    {
        var parts = path.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 0)
            return parts[0];
        else
            throw new ArgumentException("Error : GetApiNameFromPath : {Path}", nameof(path));
    }

    private static string GetRouteFromPath(string path)
    {
        var parts = path.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 1)
            return String.Join("/", parts[1..]);
        else
            throw new ArgumentException("Error : GetRouteFromPath : {Path}", nameof(path));
    }
}
