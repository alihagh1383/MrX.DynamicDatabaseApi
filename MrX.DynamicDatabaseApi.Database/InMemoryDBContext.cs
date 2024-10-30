using Microsoft.EntityFrameworkCore;

namespace MrX.DynamicDatabaseApi.Database
{
    public class InMemoryDBContext : DbContext
    {
        public DbSet<Table.InMemory.LoginsTables> Logins { get; set; }
    }
}