using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementation;
/// <summary>
/// Класс для работы тикета 
/// </summary>
public class UserAdvertRepository : BaseRepository<UserAdvert>, IUserAdvertRepository
{
    public UserAdvertRepository(ApplicationContext context) : base(context)
    {
    }
    // <summary>
    //Получение Тикета вместе с вложениями
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserAdvert?> GetAdvertstWithAttachmentsAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.UserAdverts
            .Include(ua => ua.Advert)
            .ThenInclude(a => a.Attachments) // Важно: ThenInclude!
            .FirstOrDefaultAsync(ua => ua.Id == requestId);
    }
    
}