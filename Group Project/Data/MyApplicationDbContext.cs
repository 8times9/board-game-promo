using Microsoft.EntityFrameworkCore;
using Group_Project.Data.Entities;


namespace Group_Project.Data
{
    public class MyApplicationDbContext : DbContext
    {
        public MyApplicationDbContext(DbContextOptions<MyApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Post> Post { get; set; }

        public DbSet<Emails> Emails { get; set; } 
    }
}
