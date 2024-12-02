using System.Linq.Expressions;
using MrX.DynamicDatabaseApi.Database;
using MrX.DynamicDatabaseApi.Database.Table.Dynamic;

namespace MrX.DynamicDatabaseApi.Worker;

public class DBWDynamicTable
{
    private readonly DynamicDbContext DB;

    public DBWDynamicTable(string Name)
    {
        if (Static.Ddbcs.TryGetValue(Name, out var D))
            DB = D;
        else DB = Static.Ddbcs[Name] = new DynamicDbContext(Name, Static.ConStr!);
    }

    public bool Add(DynamicTable User)
    {
        return Do(() =>
        {
            DB.Set<DynamicTable>().Add(User);
            DB.SaveChanges();
        });
    }

    public bool AddRnge(IEnumerable<DynamicTable> User)
    {
        return Do(() =>
        {
            DB.Set<DynamicTable>().AddRange(User);
            DB.SaveChanges();
        });
    }


    public bool Exist(Expression<Func<DynamicTable, bool>> F)
    {
        return DB.Set<DynamicTable>().Any(F);
    }

    public DynamicTable? First(Expression<Func<DynamicTable, bool>> F)
    {
        return DB.Set<DynamicTable>().First(F);
    }

    public DynamicTable? Find(params object?[]? Key)
    {
        return DB.Set<DynamicTable>().Find(Key);
    }

    public IEnumerable<DynamicTable> GetAll()
    {
        return DB.Set<DynamicTable>().AsEnumerable();
    }

    public bool Remove(DynamicTable User)
    {
        return Do(() =>
        {
            User.IsDeleted = true;
            DB.Set<DynamicTable>().Update(User);
            DB.SaveChanges();
        });
    }

    public bool Update(DynamicTable User)
    {
        return Do(() =>
        {
            DB.Set<DynamicTable>().Update(User);
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