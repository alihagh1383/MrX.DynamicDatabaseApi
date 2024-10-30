using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MrX.Web.Logger;
using Newtonsoft.Json;

namespace MrX.DynamicDatabaseApi.Database;

public class DynamicDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly string _tableName;

    public DynamicDbContext(string connectionString, string tableName) : base()
    {
        this._connectionString = connectionString;
        this._tableName = tableName;
        Database.ExecuteSqlRaw($"""
                                IF OBJECT_ID(N'[{tableName}]') IS NULL
                                BEGIN
                                    CREATE TABLE [{tableName}] (
                                    [Id] uniqueidentifier NOT NULL,
                                    [Properties] nvarchar(max) NULL,
                                    [CreatedDate] nvarchar(max) NULL,
                                    [IsDeleted] bit NULL,
                                    [LastUpdate] uniqueidentifier NULL,
                                    CONSTRAINT [PK_{tableName}] PRIMARY KEY ([Id])
                                );
                                END;
                                """);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Table.Dynamic.DynamicTable>().ToTable(_tableName);
        modelBuilder.Entity<Table.Dynamic.DynamicTable>().Property(p => p.DynamicColumns).HasConversion(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v) ?? new Dictionary<string, string>());
        modelBuilder.Entity<Table.Dynamic.DynamicTable>().Property(p => p.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Table.Dynamic.DynamicTable>().HasQueryFilter(p => p.IsDeleted == false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(connectionString: this._connectionString);
    }

    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();
        foreach (var item in ChangeTracker.Entries<Table.Dynamic.DynamicTable>()
                     .Where(e => e.State == EntityState.Modified))
        {
            _ = new DbUpdateLogger(this._tableName, item.Entity.Id.ToString(), item.OriginalValues);
            item.Property(p => p.LastUpdate).CurrentValue = Guid.NewGuid();
        }
        return base.SaveChanges();
    }
}