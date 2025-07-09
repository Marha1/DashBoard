using Microsoft.AspNetCore.Http;

namespace Application.Dtos.CategoryDtos
{
    
    public record CategoryCreateDto(string Name, IFormFile? Image, Guid? ParentId);
    public record CategoryUpdateDto(Guid Id, string Name, IFormFile? Image, Guid? ParentId);
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ParentId { get; set; }
    }
}