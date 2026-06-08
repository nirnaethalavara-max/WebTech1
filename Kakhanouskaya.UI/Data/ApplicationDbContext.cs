using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Kakhanouskaya.DOMAIN.Entities;

namespace Kakhanouskaya.UI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Kakhanouskaya.DOMAIN.Entities.Dish> Dish { get; set; } = default!;
    }
}
