using buoi2_PhamDucThang_2122110019.Models; 
using Microsoft.EntityFrameworkCore;

namespace buoi2_PhamDucThang_2122110019.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
