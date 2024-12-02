using Microsoft.EntityFrameworkCore;
using MrX.DynamicDatabaseApi.Database.Table.InMemory;

namespace MrX.DynamicDatabaseApi.Database;

public class InMemoryDBContext : DbContext
{
    public InMemoryDBContext(DbContextOptions<InMemoryDBContext> options) : base(options)
    {
    }

    public DbSet<LoginsTables> Logins { get; set; }
}