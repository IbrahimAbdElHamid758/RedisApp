using Microsoft.EntityFrameworkCore;
using RedisApp.Models;

namespace RedisApp.Data
{
    public class DbContextClass :DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> options):base(options)
        {}
        public DbSet<Product> Products { get; set; }
    }
}