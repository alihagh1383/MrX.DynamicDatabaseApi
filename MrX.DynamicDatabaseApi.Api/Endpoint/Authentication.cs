using MrX.DynamicDatabaseApi.CallBack;
using MrX.DynamicDatabaseApi.Worker;

namespace MrX.DynamicDatabaseApi.Api.Endpoint
{
    public static class Authentication
    {
        public static async Task<IResult> Login(string UserNameOrEmail, string Password,DBWUser UM, DBWLogins SM, HttpContext context)
        {
            IEnumerable<Database.Table.SQL.UsersTable> u = UM.GetAll().AsEnumerable().Where(p => p.CheckIsThisUser(UserNameOrEmail, Password));
            if (u.Any())
            {
                Database.Table.SQL.UsersTable? user = u?.FirstOrDefault();
                IEnumerable<Database.Table.InMemory.LoginsTables> ss = SM.GetAll().Where(p => p.IsThisSession(user.UserName, user.Password));
                if (ss.Any())
                {
                    Database.Table.InMemory.LoginsTables? session = ss?.FirstOrDefault();
                    if (session != null)
                    {
                        session.Expire = DateTimeOffset.Now.AddDays(1);
                        session!.UserLastUpdate = user?.LastUpdate;
                        SM.Update(session);
                        return await Task.FromResult(Results.Ok(Return.Sucsses(Return.Loc.Endpoint,"Session Updated", SuccessesReturnType(session))));
                    }
                }
                {
                    var session = new Database.Table.InMemory.LoginsTables() { Password = user.Password, Username = user.UserName, UserLastUpdate = user.LastUpdate };
                    SM.Add(session);
                    return await Task.FromResult(Results.Ok(Return.Sucsses(Return.Loc.Endpoint,"Session Created", SuccessesReturnType(session))));
                }
            }
            return await Task.FromResult(Results.NotFound(Return.NotFound(Return.Loc.Endpoint,"Error On Find User")));
            object SuccessesReturnType(Database.Table.InMemory.LoginsTables session) => new { GUID = session.Id, EXPIRE = session.Expire };

        }


    }
}
