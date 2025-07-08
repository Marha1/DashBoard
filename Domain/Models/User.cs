namespace Domain.Models;

public class User : AppUser
{
    public virtual ICollection<UserAdvert> UserAdverts { get; set; } = new List<UserAdvert>();
}