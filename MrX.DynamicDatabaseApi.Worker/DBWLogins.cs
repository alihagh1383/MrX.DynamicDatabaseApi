using System.Linq.Expressions;
using MrX.DynamicDatabaseApi.Database;
using MrX.DynamicDatabaseApi.Database.Table.InMemory;

namespace MrX.DynamicDatabaseApi.Worker;

public class DBWLogins
{
    private readonly SQLDBContext DB;
    private readonly InMemoryDBContext DBCMemory;

    public DBWLogins(SQLDBContext DB, InMemoryDBContext DBCMemory)
    {
        this.DBCMemory = DBCMemory;
        this.DB = DB;
    }

    public bool sync()
    {
        return Do(() =>
        {
            DBCMemory.Logins.AddRange(DB.LoginsTable);
            DBCMemory.SaveChanges();
        });
    }

    public bool Add(LoginsTables User)
    {
        return Do(() =>
        {
            DB.LoginsTable.Add(User);
            DB.SaveChanges();
            DBCMemory.Logins.Add(User);
            DBCMemory.SaveChanges();
        });
    }

    public bool AddRnge(IEnumerable<LoginsTables> User)
    {
        return Do(() =>
        {
            DB.LoginsTable.AddRange(User);
            DB.SaveChanges();
            DBCMemory.Logins.AddRange(User);
            DBCMemory.SaveChanges();
        });
    }

    public bool Exist(Expression<Func<LoginsTables, bool>> F)
    {
        return DBCMemory.Logins.Any(F);
    }

    public LoginsTables? Find(params object?[]? Key)
    {
        return DBCMemory.Logins.Find(Key);
    }

    public IEnumerable<LoginsTables> GetAll()
    {
        return DBCMemory.Logins.AsEnumerable();
    }

    public bool Remove(LoginsTables User)
    {
        return Do(() =>
        {
            DB.LoginsTable.Remove(User);
            DB.SaveChanges();
            DBCMemory.Logins.Remove(User);
            DBCMemory.SaveChanges();
        });
    }

    public bool Update(LoginsTables User)
    {
        return Do(() =>
        {
            DB.LoginsTable.Update(User);
            DB.SaveChanges();
            DBCMemory.Logins.Update(User);
            DBCMemory.SaveChanges();
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