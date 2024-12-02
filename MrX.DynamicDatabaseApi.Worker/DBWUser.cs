using System.Linq.Expressions;
using MrX.DynamicDatabaseApi.Database;
using MrX.DynamicDatabaseApi.Database.Table.SQL;

namespace MrX.DynamicDatabaseApi.Worker;

public class DBWUser
{
    private readonly SQLDBContext DB;

    public DBWUser(SQLDBContext context)
    {
        DB = context;
    }

    public bool Add(UsersTable User)
    {
        return Do(() =>
        {
            DB.UsersTable.Add(User);
            DB.SaveChanges();
        });
    }

    public bool AddRnge(IEnumerable<UsersTable> User)
    {
        return Do(() =>
        {
            DB.UsersTable.AddRange(User);
            DB.SaveChanges();
        });
    }

    public bool Remove(UsersTable User)
    {
        return Do(() =>
        {
            User.IsDeleted = true;
            DB.UsersTable.Update(User);
            DB.SaveChanges();
        });
    }

    public bool Update(UsersTable User)
    {
        return Do(() =>
        {
            DB.UsersTable.Update(User);
            DB.SaveChanges();
        });
    }

    public bool Exist(Expression<Func<UsersTable, bool>> F)
    {
        return DB.UsersTable.Any(F);
    }

    public UsersTable? First(Expression<Func<UsersTable, bool>> F)
    {
        return DB.UsersTable.First(F);
    }

    public UsersTable? Find(Expression<Func<UsersTable, bool>> F)
    {
        return DB.UsersTable.First(F);
    }

    public UsersTable? Find(params object?[]? Key)
    {
        return DB.UsersTable.Find(Key);
    }

    public IEnumerable<UsersTable> GetAll()
    {
        return DB.UsersTable.AsEnumerable();
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