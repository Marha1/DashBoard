using Domain.ValueObject;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models;


public abstract class AppUser : IdentityUser<Guid> 
{
    public FullName FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public override bool Equals(object? obj) => obj is AppUser entity && Id == entity.Id;
    public override int GetHashCode() => HashCode.Combine(Id);
}