using Application.Dtos.AdvertDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping
{
    public class AdvertMappingProfile : Profile
    {
        public AdvertMappingProfile()
        {
            // CreateAdvertDto -> Advert
            CreateMap<CreateAdvertDto, Advert>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Attachments, opt => opt.Ignore())
                .ForMember(dest => dest.City, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Advert -> AdvertDetailsDto
            CreateMap<Advert, AdvertDetailsDto>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.Name : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                    src.Attachments != null ? src.Attachments.Select(a => a.FilePath) : Enumerable.Empty<string>()));

            // Advert -> AdvertShortInfoDto
            CreateMap<Advert, AdvertShortInfoDto>()
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => 
                    src.Attachments != null && src.Attachments.Any() ? src.Attachments.First().FilePath : null))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.Name : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            // Advert -> UpdateAdvertDto
            CreateMap<Advert, UpdateAdvertDto>()
                .ForMember(dest => dest.NewImages, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedImageIds, opt => opt.Ignore())
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.City != null ? src.City.Id : Guid.Empty))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category != null ? src.Category.Id : Guid.Empty));

            // UpdateAdvertDto -> Advert
            CreateMap<UpdateAdvertDto, Advert>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Attachments, opt => opt.Ignore())
                .ForMember(dest => dest.City, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}