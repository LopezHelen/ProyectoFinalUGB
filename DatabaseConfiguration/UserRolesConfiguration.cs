using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.DatabaseConfiguration
{
    public class UserRolesConfiguration : IEntityTypeConfiguration<user_roles>
    {
        public void Configure(EntityTypeBuilder<user_roles> builder)
        {
            builder.HasKey(x => x.id);
            builder.HasIndex(x => new { x.user_id, x.role_id }).IsUnique();

            builder.HasOne(x => x.user)
                .WithMany(x => x.user_roles)
                .HasForeignKey(x => x.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.role)
                .WithMany(x => x.user_roles)
                .HasForeignKey(x => x.role_id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
