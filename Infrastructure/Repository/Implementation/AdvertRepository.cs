using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;

public class AdvertRepository : BaseRepository<Advert>, IAdvertRepository
{
    public AdvertRepository(ApplicationContext context) : base(context) { }

    public async Task<List<Advert>> GetLatestAsync(int count)
    {
        return await _context.Adverts
            .Include(a => a.City)
            .Include(a => a.Category)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Advert>> GetByCategoryAsync(Guid categoryId)
    {
        return await _context.Adverts
            .Where(a => a.CategoryId == categoryId)
            .Include(a => a.City)
            .ToListAsync();
    }
    public async Task<List<Advert>> GetByCityAsync(Guid cityId)
    {
        return await _context.Adverts
            .Where(a => a.CityId == cityId)
            .Include(a => a.City)
            .Include(a => a.Category)
            .Include(a => a.Attachments)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Advert>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Adverts
            .Where(a => a.UserId == userId)
            .Include(a => a.City)
            .Include(a => a.Category)
            .Include(a => a.Attachments)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Advert?> GetWithDetailsAsync(Guid advertId)
    {
        return await _context.Adverts
            .Include(a => a.City)
            .Include(a => a.Category)
            .Include(a => a.Attachments)
            .FirstOrDefaultAsync(a => a.Id == advertId);
    }
}