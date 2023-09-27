using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Models
{
    public class CitiesContext : DbContext
    {
        public CitiesContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<City> Cities { get; set; }
    }
}
