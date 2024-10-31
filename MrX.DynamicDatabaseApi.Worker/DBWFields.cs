using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MrX.DynamicDatabaseApi.Database.Table.Dynamic;
using MrX.DynamicDatabaseApi.Database.Table.SQL;

namespace MrX.DynamicDatabaseApi.Worker;

public class DBWFields
{
    public Database.SQLDBContext DB;
    public DBWFields(Database.SQLDBContext context)
    {
        DB = context;
    }
    public bool Add(FieldsTable User)
        => Do(() => { DB.FieldsTable.Add(User); DB.SaveChanges(); });

    public bool AddRnge(IEnumerable<FieldsTable> User)
        => Do(() => { foreach (var item in User) { DB.FieldsTable.Add(item); DB.SaveChanges(); } });

    public bool Exist(Expression<Func<FieldsTable, bool>> F)
        => DB.FieldsTable.Any(F);

    public FieldsTable? Find(params object?[]? Key)
        => DB.FieldsTable.Find(Key);

    public IEnumerable<FieldsTable> GetAll()
        => DB.FieldsTable.AsEnumerable();

    public bool Remove(FieldsTable User)
        => Do(() =>
        {
            User.IsDeleted = true;
            if (Static.Ddbcs.TryGetValue(User.Table.Name, out Database.DynamicDbContext? D)) { }
            else D = Static.Ddbcs[User.Table.Name] = new Database.DynamicDbContext(User.Table.Name, DB.Database.GetConnectionString()!);
            var g = Guid.NewGuid();
            D.Set<DynamicTable>().ForEachAsync(p => { p.DynamicColumns?.Add("!" + g + "!" + User.Name, p.DynamicColumns[User.Name]); p.DynamicColumns?.Remove(User.Name); });
            DB.FieldsTable.Update(User);
            DB.SaveChanges();
        });

    public bool Update(FieldsTable User)
        => Do(() => { DB.FieldsTable.Update(User); DB.SaveChanges(); });

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