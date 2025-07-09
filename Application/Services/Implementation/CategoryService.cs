
using Application.Dtos.CategoryDtos;
using AutoMapper;
using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Application.Services.Interfaces;

namespace Application.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IAttachmentService attachmentService,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _attachmentService = attachmentService;
            _mapper = mapper;
        }

        public async Task<Guid> CreateCategoryAsync(CategoryCreateDto dto, CancellationToken cancellationToken = default)
        {
            var category = _mapper.Map<Category>(dto);
            
            if (dto.Image != null)
            {
                var attachment = await _attachmentService.UploadAttachmentAsync(dto.Image, category.Id, cancellationToken);
                category.ImageUrl = attachment.FilePath;
            }

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return category.Id;
        }

        public async Task UpdateCategoryAsync(CategoryUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.Id, cancellationToken)
                ?? throw new KeyNotFoundException("Category not found");
            
            category.Name = dto.Name;
            category.ParentCategoryId = dto.ParentId;

            if (dto.Image != null)
            {
                var attachment = await _attachmentService.UploadAttachmentAsync(dto.Image, category.Id, cancellationToken);
                category.ImageUrl = attachment.FilePath;
            }

            await _categoryRepository.UpdateAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category, cancellationToken);
                await _categoryRepository.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<List<CategoryDto>> GetSubcategoriesAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetSubcategoriesAsync(parentId, cancellationToken);
            return _mapper.Map<List<CategoryDto>>(categories);
        }
    }
}