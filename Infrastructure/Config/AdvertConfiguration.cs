using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;
public class AdvertConfiguration : IEntityTypeConfiguration<Advert>
{
    public void Configure(EntityTypeBuilder<Advert> builder)
    {
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.Price).IsRequired();
            
        builder.Property(a => a.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);
            
      
        builder.HasOne(a => a.City)
            .WithMany(c => c.Adverts)
            .HasForeignKey(a => a.CityId);
            
        builder.HasOne(a => a.Category)
            .WithMany(c => c.Adverts)
            .HasForeignKey(a => a.CategoryId);
    }
}