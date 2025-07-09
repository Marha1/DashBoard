using Domain.Models;

namespace Domain.Interfaces.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetSubcategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);
    }
}