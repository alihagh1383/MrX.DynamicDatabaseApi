using System.Linq.Expressions;
using MrX.DynamicDatabaseApi.Database.Table.SQL;

namespace MrX.DynamicDatabaseApi.Worker;

public class DBWUser
{
    private Database.SQLDBContext DB;   
    public DBWUser(Database.SQLDBContext context)
    {
        DB = context;
    }
    public bool Add(UsersTable User)
        => Do(() => { DB.UsersTable.Add(User); DB.SaveChanges(); });
    public bool AddRnge(IEnumerable<UsersTable> User)
        => Do(() => { DB.UsersTable.AddRange(User); DB.SaveChanges(); });

    public bool Remove(UsersTable User)
        => Do(() => { User.IsDeleted = true; DB.UsersTable.Update(User); DB.SaveChanges(); });

    public bool Update(UsersTable User)
        => Do(() => { DB.UsersTable.Update(User); DB.SaveChanges(); });

    public bool Exist(Expression<Func<UsersTable, bool>> F)
        => DB.UsersTable.Any(F);
    public UsersTable? First(Expression<Func<UsersTable, bool>> F)
        => DB.UsersTable.First(F);
    public UsersTable? Find(Expression<Func<UsersTable, bool>> F)
        => DB.UsersTable.First(F);

    public UsersTable? Find(params object?[]? Key)
        => DB.UsersTable.Find(Key);

    public IEnumerable<UsersTable> GetAll()
        => DB.UsersTable.AsEnumerable();

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