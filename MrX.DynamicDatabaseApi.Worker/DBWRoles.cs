using MrX.DynamicDatabaseApi.Database.Table.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MrX.DynamicDatabaseApi.Worker
{
    public class DBWRoles
    {
        public Database.SQLDBContext DB;
        public DBWRoles(Database.SQLDBContext context)
        {
            DB = context;
        }
        public bool Add(RolesTable User)
    => Do(() => { DB.RolesTable.Add(User); DB.SaveChanges(); });

        public bool AddRnge(IEnumerable<RolesTable> User)
        => Do(() => { foreach (var item in User) { DB.RolesTable.Add(item); DB.SaveChanges(); } });

        public bool Exist(Expression<Func<RolesTable, bool>> F)
        => DB.RolesTable.Any(F);
        public RolesTable? First(Expression<Func<RolesTable, bool>> F)
        => DB.RolesTable.First(F);

        public RolesTable? Find(params object?[]? Key)
        => DB.RolesTable.Find(Key);

        public IEnumerable<RolesTable> GetAll()
        => DB.RolesTable.AsEnumerable();

        public bool Remove(RolesTable User)
        => Do(() =>
        { User.IsDeleted = true; DB.RolesTable.Update(User); DB.SaveChanges(); });

        public bool Update(RolesTable User)
        => Do(() => { DB.RolesTable.Update(User); DB.SaveChanges(); });

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
