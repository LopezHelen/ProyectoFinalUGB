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
            builder.Property(x => x.id).HasColumnName("id_user");

            builder.HasIndex(x => x.email).IsUnique();
            builder.Property(x => x.email).IsRequired().HasMaxLength(50);
            builder.Property(x => x.salt).HasColumnName("password_salt").IsRequired();
            builder.Property(x => x.password_hash).IsRequired();
            builder.Property(x => x.email_confirmed).IsRequired();
            builder.Property(x => x.failed_login_attempts).HasColumnName("login_attempts").IsRequired();

            builder.Property(x => x.email_confirmation_token)
                .HasColumnName("email_confirmation_token")
                .HasConversion(
                    v => v.HasValue ? v.Value.ToString() : null,
                    v => string.IsNullOrEmpty(v) ? (Guid?)null : Guid.Parse(v))
                .HasMaxLength(100);
            builder.HasIndex(x => x.email_confirmation_token);

            builder.Property(x => x.first_name).HasMaxLength(30);
            builder.Property(x => x.last_name).HasMaxLength(30);
            builder.Property(x => x.address).HasMaxLength(200);
            builder.Property(x => x.dui).HasMaxLength(10);
            builder.HasIndex(x => x.dui).IsUnique();
        }
    }
}
