using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.DatabaseConfiguration
{
    public class UsersConfiguration : IEntityTypeConfiguration<users>
    {
        public void Configure(EntityTypeBuilder<users> builder)
        {
            builder.HasKey(x => x.id);

            builder.HasIndex(x => x.email).IsUnique();
            builder.Property(x => x.email).IsRequired().HasMaxLength(50);
            builder.Property(x => x.salt).IsRequired();
            builder.Property(x => x.password_hash).IsRequired();
            builder.Property(x => x.email_confirmed).IsRequired();
            builder.Property(x => x.failed_login_attempts).IsRequired();

            builder.HasIndex(x => x.email_confirmation_token);

            builder.Property(x => x.first_name).HasMaxLength(30);
            builder.Property(x => x.last_name).HasMaxLength(30);
            builder.Property(x => x.address).HasMaxLength(200);
            builder.Property(x => x.dui).HasMaxLength(10);
            builder.HasIndex(x => x.dui).IsUnique();
        }
    }
}
