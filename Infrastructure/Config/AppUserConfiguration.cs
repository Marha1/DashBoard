using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Config;
public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers");

        builder.HasKey(u => u.Id);

        builder.OwnsOne(u => u.FullName, fullName =>
        {
            fullName.Property(fn => fn.FirstName).IsRequired();
            fullName.Property(fn => fn.LastName).IsRequired();
            fullName.Property(fn => fn.MiddleName);
        });
    }
}

