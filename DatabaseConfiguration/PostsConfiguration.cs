using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.DatabaseConfiguration
{
    public class PostsConfiguration : IEntityTypeConfiguration<posts>
    {
        public void Configure(EntityTypeBuilder<posts> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.id).HasColumnName("id_post");
            builder.Property(x => x.title).IsRequired().HasMaxLength(100);
            builder.Property(x => x.content).IsRequired().HasColumnType("text");

            builder.HasOne(x => x.user)
                .WithMany()
                .HasForeignKey(x => x.user_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
