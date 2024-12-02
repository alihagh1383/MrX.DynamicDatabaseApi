using System.Linq.Expressions;
using MrX.DynamicDatabaseApi.Database;
using MrX.DynamicDatabaseApi.Database.Table.SQL;

namespace MrX.DynamicDatabaseApi.Worker;

public class DBWRoles
{
    public SQLDBContext DB;

    public DBWRoles(SQLDBContext context)
    {
        DB = context;
    }

    public bool Add(RolesTable User)
    {
        return Do(() =>
        {
            DB.RolesTable.Add(User);
            DB.SaveChanges();
        });
    }

    public bool AddRnge(IEnumerable<RolesTable> User)
    {
        return Do(() =>
        {
            foreach (var item in User)
            {
                DB.RolesTable.Add(item);
                DB.SaveChanges();
            }
        });
    }

    public bool Exist(Expression<Func<RolesTable, bool>> F)
    {
        return DB.RolesTable.Any(F);
    }

    public RolesTable? First(Expression<Func<RolesTable, bool>> F)
    {
        return DB.RolesTable.First(F);
    }

    public RolesTable? Find(params object?[]? Key)
    {
        return DB.RolesTable.Find(Key);
    }

    public IEnumerable<RolesTable> GetAll()
    {
        return DB.RolesTable.AsEnumerable();
    }

    public bool Remove(RolesTable User)
    {
        return Do(() =>
        {
            User.IsDeleted = true;
            DB.RolesTable.Update(User);
            DB.SaveChanges();
        });
    }

    public bool Update(RolesTable User)
    {
        return Do(() =>
        {
            DB.RolesTable.Update(User);
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