using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;

public class CityRepository : BaseRepository<City>, ICityRepository
{
    public CityRepository(ApplicationContext context) : base(context)
    {
    }

    public async Task<List<City>> GetActiveCitiesAsync()
    {
        return await _context.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Cities
            .AnyAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task ToggleActivityAsync(Guid cityId)
    {
        var city = await _context.Cities.FindAsync(cityId);
        if (city != null)
        {
            city.IsActive = !city.IsActive;
            await _context.SaveChangesAsync();
        }
    }
}