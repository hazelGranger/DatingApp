using DatingApp.API.Model.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}

        // tell dbcontext about our entities
        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }
    }
}