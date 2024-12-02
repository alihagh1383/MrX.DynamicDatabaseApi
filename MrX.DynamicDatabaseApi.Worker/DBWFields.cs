using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MrX.DynamicDatabaseApi.Database;
using MrX.DynamicDatabaseApi.Database.Table.Dynamic;
using MrX.DynamicDatabaseApi.Database.Table.SQL;

namespace MrX.DynamicDatabaseApi.Worker;

public class DBWFields
{
    public SQLDBContext DB;

    public DBWFields(SQLDBContext context)
    {
        DB = context;
    }

    public bool Add(FieldsTable User)
    {
        return Do(() =>
        {
            DB.FieldsTable.Add(User);
            DB.SaveChanges();
        });
    }

    public bool AddRnge(IEnumerable<FieldsTable> User)
    {
        return Do(() =>
        {
            foreach (var item in User)
            {
                DB.FieldsTable.Add(item);
                DB.SaveChanges();
            }
        });
    }

    public bool Exist(Expression<Func<FieldsTable, bool>> F)
    {
        return DB.FieldsTable.Any(F);
    }

    public FieldsTable? Find(params object?[]? Key)
    {
        return DB.FieldsTable.Find(Key);
    }

    public IEnumerable<FieldsTable> GetAll()
    {
        return DB.FieldsTable.AsEnumerable();
    }

    public bool Remove(FieldsTable User)
    {
        return Do(() =>
        {
            User.IsDeleted = true;
            if (Static.Ddbcs.TryGetValue(User.Table.Name, out var D))
            {
            }
            else
            {
                D = Static.Ddbcs[User.Table.Name] =
                    new DynamicDbContext(User.Table.Name, DB.Database.GetConnectionString()!);
            }

            var g = Guid.NewGuid();
            D.Set<DynamicTable>().ForEachAsync(p =>
            {
                p.DynamicColumns?.Add("!" + g + "!" + User.Name, p.DynamicColumns[User.Name]);
                p.DynamicColumns?.Remove(User.Name);
            });
            DB.FieldsTable.Update(User);
            DB.SaveChanges();
        });
    }

    public bool Update(FieldsTable User)
    {
        return Do(() =>
        {
            DB.FieldsTable.Update(User);
            DB.SaveChanges();
        });
    }

    private bool Do(Action A)
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