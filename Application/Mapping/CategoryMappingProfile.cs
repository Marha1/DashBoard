using Application.Dtos.CategoryDtos;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // CategoryCreateDto -> Category
            CreateMap<CategoryCreateDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Subcategories, opt => opt.Ignore())
                .ForMember(dest => dest.Adverts, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore());

            // CategoryUpdateDto -> Category
            CreateMap<CategoryUpdateDto, Category>()
                .ForMember(dest => dest.Subcategories, opt => opt.Ignore())
                .ForMember(dest => dest.Adverts, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore());

            // Category -> CategoryDto
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
        }
    }
}