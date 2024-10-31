using MrX.DynamicDatabaseApi.Database.Table.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MrX.DynamicDatabaseApi.Worker
{
    public class DBWDynamicTable
    {
        private Database.DynamicDbContext DB;

        public DBWDynamicTable(string Name)
        {
            if (Static.Ddbcs.TryGetValue(Name, out Database.DynamicDbContext? D)) { DB = D; }
            else DB = Static.Ddbcs[Name] = new Database.DynamicDbContext(Name, Static.ConStr!);
        }
        public bool Add(DynamicTable User)
       => Do(() => { DB.Set<DynamicTable>().Add(User); DB.SaveChanges(); });

        public bool AddRnge(IEnumerable<DynamicTable> User)
        => Do(() => { DB.Set<DynamicTable>().AddRange(User); DB.SaveChanges(); });


        public bool Exist(Expression<Func<DynamicTable, bool>> F)
        => DB.Set<DynamicTable>().Any(F);
        public DynamicTable? First(Expression<Func<DynamicTable, bool>> F)
        => DB.Set<DynamicTable>().First(F);

        public DynamicTable? Find(params object?[]? Key)
        => DB.Set<DynamicTable>().Find(Key);
        public IEnumerable<DynamicTable> GetAll()
        => DB.Set<DynamicTable>().AsEnumerable();

        public bool Remove(DynamicTable User)
        => Do(() => { User.IsDeleted = true; DB.Set<DynamicTable>().Update(User); DB.SaveChanges(); });

        public bool Update(DynamicTable User)
        => Do(() => { DB.Set<DynamicTable>().Update(User); DB.SaveChanges(); });
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
