using Microsoft.EntityFrameworkCore;
using UGB.MVC.Entities;

namespace UGB.MVC.Aplicaciones.Seguras.Entities
{
    public class StoreCTX : DbContext
    {
        public StoreCTX(DbContextOptions<StoreCTX> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreCTX).Assembly);
        }

        public DbSet<users> users { get; set; }
        public DbSet<roles> roles { get; set; }
        public DbSet<user_roles> user_roles { get; set; }
        public DbSet<posts> posts { get; set; }
        public DbSet<comments> comments { get; set; }
        public DbSet<post_pictures> post_pictures { get; set; }
    }
}
