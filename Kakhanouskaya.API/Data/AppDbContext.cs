using Microsoft.EntityFrameworkCore;

using Kakhanouskaya.DOMAIN.Entities; // Твае сутнасці

namespace Kakhanouskaya.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
