using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UGB.MVC.Aplicaciones.Seguras.Entities
{
    public class StoreCTXFactory : IDesignTimeDbContextFactory<StoreCTX>
    {
        public StoreCTX CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<StoreCTX>();
            optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

            return new StoreCTX(optionsBuilder.Options);
        }
    }
}
