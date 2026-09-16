using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.DatabaseConfiguration
{
    public class RolesConfiguration : IEntityTypeConfiguration<roles>
    {
        public void Configure(EntityTypeBuilder<roles> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.id).HasColumnName("id_role");
            builder.Property(x => x.name).HasColumnName("description").IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.name).IsUnique();

            builder.HasData(
                new roles { id = 1, name = "Administrator" },
                new roles { id = 2, name = "User" }
            );
        }
    }
}
