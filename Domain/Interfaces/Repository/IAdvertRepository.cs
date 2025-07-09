using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IAdvertRepository : IRepository<Advert>
{
    Task<List<Advert>> GetLatestAsync(int count);
    Task<List<Advert>> GetByCategoryAsync(Guid categoryId);
    Task<List<Advert>> GetByCityAsync(Guid cityId);
    Task<List<Advert>> GetByUserIdAsync(Guid userId);
    Task<Advert?> GetWithDetailsAsync(Guid advertId);
    Task<List<Advert>> GetLatestAsync(int count, Guid? categoryId = null, Guid? cityId = null);

}