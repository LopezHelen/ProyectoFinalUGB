using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.DatabaseConfiguration
{
    public class CommentsConfiguration : IEntityTypeConfiguration<comments>
    {
        public void Configure(EntityTypeBuilder<comments> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.content).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.user)
                .WithMany()
                .HasForeignKey(x => x.user_id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.post)
                .WithMany(x => x.comments)
                .HasForeignKey(x => x.post_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
