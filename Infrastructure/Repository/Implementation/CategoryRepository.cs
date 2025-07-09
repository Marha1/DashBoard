using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repository.Implementation
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationContext context) : base(context)
        {
        }

        public async Task<List<Category>> GetSubcategoriesAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .Include(c => c.Subcategories)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Category>> GetCategoriesWithSubcategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.Subcategories)
                .Where(c => c.ParentCategoryId == null) 
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}