namespace Domain.Models;

public class City
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<Advert> Adverts { get; set; } = new List<Advert>();
}