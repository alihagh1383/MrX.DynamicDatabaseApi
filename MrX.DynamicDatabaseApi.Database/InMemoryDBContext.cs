using Microsoft.EntityFrameworkCore;

namespace MrX.DynamicDatabaseApi.Database
{
    public class InMemoryDBContext : DbContext
    {
        public InMemoryDBContext(DbContextOptions<InMemoryDBContext> options) : base(options) 
        {
            
        }
        public DbSet<Table.InMemory.LoginsTables> Logins { get; set; }
    }
}