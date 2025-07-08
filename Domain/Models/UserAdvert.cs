
using Domain.Primitives;

namespace Domain.Models;

public class UserAdvert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary> Статус объявления (активно, на модерации и т.д.) </summary>
    public AdvertStatus Status { get; set; } = AdvertStatus.Active;
    
    /// <summary> Дата создания связи </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary> Флаг избранного </summary>
    public bool IsFavorite { get; set; }

    // Связи
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid AdvertId { get; set; }
    public Advert Advert { get; set; }
}

