using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.DatabaseConfiguration
{
    public class PostPicturesConfiguration : IEntityTypeConfiguration<post_pictures>
    {
        public void Configure(EntityTypeBuilder<post_pictures> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.id).HasColumnName("id_post_picture");
            builder.Property(x => x.file_original_name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.file_name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.file_hash).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.post)
                .WithMany(x => x.post_pictures)
                .HasForeignKey(x => x.post_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
