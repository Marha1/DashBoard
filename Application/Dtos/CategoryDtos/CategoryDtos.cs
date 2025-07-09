using Microsoft.AspNetCore.Http;

namespace Application.Dtos.CategoryDtos
{
    public record CategoryCreateDto(string Name, IFormFile? Image, Guid? ParentId);
    public record CategoryUpdateDto(Guid Id, string Name, IFormFile? Image, Guid? ParentId);
    public record CategoryDto(Guid Id, string Name, string? ImageUrl, Guid? ParentId);
}