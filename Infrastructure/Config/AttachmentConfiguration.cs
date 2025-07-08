using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(a => a.FilePath)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(a => a.UploadDate)
            .HasDefaultValueSql("NOW()");
            
        builder.HasOne(a => a.Advert)
            .WithMany(a => a.Attachments)
            .HasForeignKey(a => a.AdvertId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}