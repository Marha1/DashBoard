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
                .ForMember(dest => dest.Attachments, opt => opt.Ignore())
                .ForMember(dest => dest.City, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Advert -> AdvertDetailsDto
            CreateMap<Advert, AdvertDetailsDto>()
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Attachments.Select(a => a.FilePath)));

            // Advert -> AdvertShortInfoDto
            CreateMap<Advert, AdvertShortInfoDto>()
                .ForMember(dest => dest.MainImageUrl, 
                    opt => opt.MapFrom(src => src.Attachments.FirstOrDefault().FilePath));
            // Добавим новый маппинг для UpdateAdvertDto
            CreateMap<Advert, UpdateAdvertDto>()
                .ForMember(dest => dest.NewImages, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedImageIds, opt => opt.Ignore());
        }
    }
}