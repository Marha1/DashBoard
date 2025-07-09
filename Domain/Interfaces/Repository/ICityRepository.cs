using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface ICityRepository : IRepository<City>
{
    Task<List<City>> GetActiveCitiesAsync();
    Task<bool> ExistsByNameAsync(string name);
    Task ToggleActivityAsync(Guid cityId);
}
