using Microsoft.EntityFrameworkCore;
using MrX.DynamicDatabaseApi.Database.Table.Dynamic;
using MrX.Web.Logger;
using Newtonsoft.Json;

namespace MrX.DynamicDatabaseApi.Database;

public class DynamicDbContext : DbContext
{
    private readonly string _connectionString;
    private readonly string _tableName;

    public DynamicDbContext(string tableName, string connectionString)
    {
        _connectionString = connectionString;
        _tableName = tableName;
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
        modelBuilder.Entity<DynamicTable>().ToTable(_tableName);
        modelBuilder.Entity<DynamicTable>().Property(p => p.DynamicColumns).HasConversion(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v) ?? new Dictionary<string, string>());
        modelBuilder.Entity<DynamicTable>().Property(p => p.CreatedDate)
            .HasConversion(p => p.ToString(), v => DateTime.Parse(v));
        modelBuilder.Entity<DynamicTable>().Property(p => p.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<DynamicTable>().HasQueryFilter(p => p.IsDeleted == false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseMySql(ServerVersion.AutoDetect(_connectionString));
    }

    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();
        foreach (var item in ChangeTracker.Entries<DynamicTable>()
                     .Where(e => e.State == EntityState.Modified))
        {
            _ = new DbUpdateLogger(_tableName, item.Entity.Id.ToString(), item.OriginalValues);
            item.Property(p => p.LastUpdate).CurrentValue = Guid.NewGuid();
        }

        return base.SaveChanges();
    }
}