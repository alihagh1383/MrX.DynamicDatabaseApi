using MrX.DynamicDatabaseApi.Database.Table.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MrX.DynamicDatabaseApi.Worker
{
    public class DBWLogins
    {
        private Database.InMemoryDBContext DBCMemory;
        private Database.SQLDBContext DB;
        public DBWLogins(Database.SQLDBContext DB, Database.InMemoryDBContext DBCMemory)
        {
            this.DBCMemory = DBCMemory;
            this.DB = DB;
        }
        
        public bool sync()
            => Do(() =>
            {
                DBCMemory.Logins.AddRange(DB.LoginsTable);
                DBCMemory.SaveChanges();
            });

        public bool Add(LoginsTables User)
        => Do(() =>
        {
            DB.LoginsTable.Add(User);
            DB.SaveChanges();
            DBCMemory.Logins.Add(User);
            DBCMemory.SaveChanges();
        });

        public bool AddRnge(IEnumerable<LoginsTables> User)
         => Do(() =>
         {
             DB.LoginsTable.AddRange(User);
             DB.SaveChanges();
             DBCMemory.Logins.AddRange(User);
             DBCMemory.SaveChanges();
         });

        public bool Exist(Expression<Func<LoginsTables, bool>> F)
        => DBCMemory.Logins.Any(F);

        public LoginsTables? Find(params object?[]? Key)
        => DBCMemory.Logins.Find(Key);

        public IEnumerable<LoginsTables> GetAll()
        => DBCMemory.Logins.AsEnumerable();

        public bool Remove(LoginsTables User)
        => Do(() =>
        {
            DB.LoginsTable.Remove(User);
            DB.SaveChanges();
            DBCMemory.Logins.Remove(User);
            DBCMemory.SaveChanges();
        });

        public bool Update(LoginsTables User)
        => Do(() =>
        {
            DB.LoginsTable.Update(User);
            DB.SaveChanges();
            DBCMemory.Logins.Update(User);
            DBCMemory.SaveChanges();
        });

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
