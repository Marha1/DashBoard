
using Application.Dtos.CityDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping
{
    public class CityMappingProfile : Profile
    {
        public CityMappingProfile()
        {
            // CreateCityDto -> City
            CreateMap<CityCreateDto, City>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Adverts, opt => opt.Ignore());

            // CityUpdateDto -> City
            CreateMap<CityUpdateDto, City>()
                .ForMember(dest => dest.Adverts, opt => opt.Ignore());

            // City -> CityDto
            CreateMap<City, CityDto>();
        }
    }
}