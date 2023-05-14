using AutoMapper;
using MockSrv.DTOs;
using MockSrv.Mapper.Transformation;

namespace MockSrv.Mapper
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<HttpContext, RequestDto>()
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Request.Path))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Request.Method))
                .ForMember(dest => dest.Headers, opt => opt.MapFrom(src => src.Request.Headers))
                .ForMember(dest => dest.QueryString, opt => opt.MapFrom(src => src.Request.QueryString.Value.Clean()))
                .ForMember(
                    dest => dest.Body,
                    opt => opt.ConvertUsing
                    (
                        new HttpBodyTransformation(),
                        src => new BodyDto 
                        {
                            ContentType = src.Request.Headers.ContentType, 
                            Body = src.Request.Body 
                        }
                    )
                );
        }
    }
}