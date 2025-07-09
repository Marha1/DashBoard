using Application.Dtos.CategoryDtos;

namespace Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Guid> CreateCategoryAsync(CategoryCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateCategoryAsync(CategoryUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
        Task<CategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CategoryDto>> GetSubcategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);
    }
}