using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

// Infrastructure/Config/UserAdvertConfiguration.cs
public class UserAdvertConfiguration : IEntityTypeConfiguration<UserAdvert>
{
    public void Configure(EntityTypeBuilder<UserAdvert> builder)
    {
        builder.ToTable("UserAdverts");
        
        builder.HasKey(ua => ua.Id);
        
        // Связь с пользователем
        builder.HasOne(ua => ua.User)
            .WithMany(u => u.UserAdverts)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Связь с объявлением
        builder.HasOne(ua => ua.Advert)
            .WithMany(a => a.UserAdverts)
            .HasForeignKey(ua => ua.AdvertId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}