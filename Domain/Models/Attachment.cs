
namespace Domain.Models;
public class Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    // Связь с объявлением
    public Guid AdvertId { get; set; }
    public virtual Advert Advert { get; set; }
}