using Microsoft.EntityFrameworkCore;
using MrX.DynamicDatabaseApi.Database.Table.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MrX.DynamicDatabaseApi.Worker
{
    public class DBWTabels
    {
        private Database.SQLDBContext DB;
        public DBWTabels(Database.SQLDBContext context)
        {
            DB = context;
        }
        public TablesTable Add(String Name, UsersTable User)
        {
            var Role = new RolesTable()
            {
                Name = "Table." + Name,
                Creator = User
            };
            var Table = new Database.Table.SQL.TablesTable()
            {
                Name = Name,
                Creator = User
            };
            Role.Users.Add(User);
            Table.UpdateRoles.Add(Role);
            Table.DeleteRoles.Add(Role);
            Table.AddRoles.Add(Role);
            Table.ReadRoles.Add(Role);
            DB.RolesTable.Add(Role);
            DB.TablesTable.Add(Table);
            DB.SaveChanges();
            Static.Ddbcs.Add(Name, new(Name, DB.Database.GetConnectionString()!));
            return Table;
        }

        public bool AddRnge(IEnumerable<TablesTable> User)
        => Do(() => { foreach (var item in User) { DB.TablesTable.Add(item); Static.Ddbcs.TryAdd(item.Name, new DBCDynamicTable(item.Name, DB.Database.GetConnectionString()!)); DB.SaveChanges(); } });

        public bool Exist(Expression<Func<TablesTable, bool>> F)
        => DB.TablesTable.Any(F);
        public TablesTable? First(Expression<Func<TablesTable, bool>> F)
        => DB.TablesTable.First(F);
        public FieldsTable? FirstField(string Table, string FieldName)
        => DB.FieldsTable.First(t => t.Name == FieldName && t.Table.Name == Table);
        public TablesTable? Find(params object?[]? Key)
        => DB.TablesTable.Find(Key);

        public IEnumerable<TablesTable> GetAll()
        => DB.TablesTable.AsEnumerable();

        public bool Remove(TablesTable T, UsersTable User)
        => Do(() =>
        {
            var R = DB.RolesTable.First(p => p.Name == "Table." + T.Name);
            if (Static.Ddbcs.TryGetValue(T.Name, out Database.DynamicDbContext? D)) { }
            else D = Static.Ddbcs[T.Name] = new Database.DynamicDbContext(T.Name, DB.Database.GetConnectionString()!);
            var G = Guid.NewGuid();
            var NewName = $"_{G}_{T.Name}";
            T.Filds.ForEach(p => { p.IsDeleted = true; });
            T.Name = NewName;
            T.IsDeleted = true;
            R.Name = $"_{G}_Table.{T.Name}";
            R.IsDeleted = true;
            DB.Database.ExecuteSqlRaw($"sp_rename '{T.Name}','{NewName}';\r\n");
            DB.Database.ExecuteSqlRaw($"sp_rename 'PK_{T.Name}','_PK_{NewName}';\r\n");
            Static.Ddbcs.Remove(T.Name);
            DB.TablesTable.Update(T);
            DB.RolesTable.Update(R);
            DB.SaveChanges();
        });

        public FieldsTable? AddField(FieldsTable Field, TablesTable T)
        {
            var R = DB.RolesTable.First(p => p.Name == "Table." + T.Name);
            T.Filds.Add(Field);
            R.FieldsAddRole.Add(Field);
            R.FieldsDeleteRole.Add(Field);
            R.FieldsUpdateRole.Add(Field);
            R.FieldsReadRole.Add(Field);
            DB.TablesTable.Update(T);
            DB.RolesTable.Update(R);
            DB.SaveChanges();
            return Field;
        }
        public FieldsTable? RemoveField(FieldsTable F, TablesTable T)
        {
            T.Filds.Remove(F);
            T.UpdateRoles.Clear();
            T.DeleteRoles.Clear();
            T.ReadRoles.Clear();
            T.AddRoles.Clear();
            F.AddRoles.Clear();
            F.ReadRoles.Clear();
            F.UpdateRoles.Clear();
            F.DeleteRoles.Clear();
            F.IsDeleted = true;
            DB.FieldsTable.Update(F);
            DB.TablesTable.Update(T);
            DB.SaveChanges();
            return F;
        }

        public FieldsTable? UpdateField(FieldsTable F)
        {
            DB.FieldsTable.Update(F);
            DB.SaveChanges();
            return F;
        }

        bool Do(Action A)
        {
            try
            {
                A.Invoke();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
