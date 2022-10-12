using Microsoft.EntityFrameworkCore;
using todo_universe.Models;

namespace todo_universe.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {

        }

        public DbSet<Todo> Todos { get; set; }
        public DbSet<User> Users{ get; set; }
    }
}
