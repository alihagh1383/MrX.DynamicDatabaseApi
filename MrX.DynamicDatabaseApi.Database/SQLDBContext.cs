using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MrX.DynamicDatabaseApi.Database.Table;
using MrX.Web.Logger;
using Newtonsoft.Json;

namespace MrX.DynamicDatabaseApi.Database
{
    public class SQLDBContext : DbContext
    {
        public SQLDBContext(DbContextOptions<SQLDBContext> options) : base(options)
        {
        // Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var stringobjectDictionaryKeyValueComparer = new ValueComparer<Dictionary<string, object>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToDictionary());

            var DateTimeComparer = new ValueComparer<List<DateTime>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            modelBuilder.Entity<Table.SQL.TablesTable>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Table.SQL.UsersTable>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Table.SQL.RolesTable>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Table.SQL.FieldsTable>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Table.InMemory.LoginsTables>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Table.SQL.TablesTable>().HasQueryFilter(p => p.IsDeleted == false);
            modelBuilder.Entity<Table.SQL.UsersTable>().HasQueryFilter(p => p.IsDeleted == false);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasQueryFilter(p => p.IsDeleted == false);
            modelBuilder.Entity<Table.SQL.FieldsTable>().HasQueryFilter(p => p.IsDeleted == false);
            modelBuilder.Entity<Table.InMemory.LoginsTables>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<Table.SQL.TablesTable>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<Table.SQL.UsersTable>().Property(p => p.Data).HasConversion(p => JsonConvert.SerializeObject(p), p => JsonConvert.DeserializeObject<Dictionary<string, object>>(p) ?? new Dictionary<string, object>(), stringobjectDictionaryKeyValueComparer);
            modelBuilder.Entity<Table.SQL.UsersTable>().HasIndex(p => p.UserName).IsUnique();

            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.FieldsAddRole).WithMany(e => e.AddRoles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.TabelsAddRole).WithMany(e => e.AddRoles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.FieldsDeleteRole).WithMany(e => e.DeleteRoles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.TabelsDeleteRole).WithMany(e => e.DeleteRoles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.FieldsReadRole).WithMany(e => e.ReadRoles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.TabelsReadRole).WithMany(e => e.ReadRoles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.FieldsUpdateRole).WithMany(e => e.UpdateRoles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.TabelsUpdateRole).WithMany(e => e.UpdateRoles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasMany(e => e.Users).WithMany(e => e.Roles);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasOne(e => e.Creator).WithMany(e => e.CreateRoles).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Table.SQL.RolesTable>().HasIndex(e => e.Name).IsUnique();
        }

        public DbSet<Table.SQL.TablesTable> TablesTable { get; set; }
        public DbSet<Table.SQL.UsersTable> UsersTable { get; set; }
        public DbSet<Table.SQL.FieldsTable> FieldsTable { get; set; }
        public DbSet<Table.InMemory.LoginsTables> LoginsTable { get; set; }
        public DbSet<Table.SQL.RolesTable> RolesTable { get; set; }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            SaveChangesSet();
            return base.SaveChanges();
        }

        public void SaveChangesSet()
        {
            //foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            //{
            //    if (item.Entity is not Tables.SessionsTable)
            //        item.Property("CreateTime").CurrentValue = DateTime.UtcNow;
            //}
            foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
            {
                if (item.Entity is BaseTable)
                {
                    _ = new DbUpdateLogger(item.Metadata.Name, ((BaseTable)item.Entity).Id.ToString(),
                        item.OriginalValues);
                    item.Property("LastUpdate").CurrentValue = Guid.NewGuid();
                }
            }
        }
    }
}