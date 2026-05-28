using AutoMapper;
using MockSrv.Application.DTOs;
using MockSrv.Web.Models;

namespace MockSrv.Web.Mapper;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<MockRequestResponseDto, MockRequestResponseModel>().ReverseMap();
    }
}