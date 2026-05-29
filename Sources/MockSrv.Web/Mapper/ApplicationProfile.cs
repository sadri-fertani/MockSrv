using AutoMapper;
using MockSrv.Web.Dtos;
using MockSrv.Web.Models;

namespace MockSrv.Web.Mapper;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<MockRequestResponseDto, MockRequestResponseModel>().ReverseMap();
    }
}