
namespace Domain.Models;
public class Advert
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ContactPhone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Связи
    public Guid CityId { get; set; }
    public virtual City City { get; set; }
    
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; }
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    
    // Коллекция вложений
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    public virtual ICollection<UserAdvert> UserAdverts { get; set; } = new List<UserAdvert>();

}