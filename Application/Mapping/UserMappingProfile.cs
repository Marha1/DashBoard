using Application.Dtos.AccountDtos;
using AutoMapper;
using Domain.Models;
using Domain.ValueObject;

namespace Application.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => new FullName(src.FirstName, src.LastName, src.Surname)));
    }
}