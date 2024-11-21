using Microsoft.AspNetCore.Identity;
using MrX.DynamicDatabaseApi.CallBack;
using MrX.DynamicDatabaseApi.Worker;

namespace MrX.DynamicDatabaseApi.Api.Endpoint
{
    public class TablesEndpoint
    {
        public static Return TablesAdd(string Name, DBWUser UM, DBWTabels TM, HttpContext http)
        {

            if (TM.Exist(t => t.Name == Name))
                return Return.ThisExist(Return.Loc.Endpoint, $"Table {Name} Exist");
            var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
            TM.Add(Name, User!);
            return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Added");
        }
        public static Return TablesGet(string Name, DBWUser UM, DBWTabels TM, HttpContext http)
        {
            var T = TM.First(t => t.Name == Name);
            if (T == null)
                return Return.ThisExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
            var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
            if (T.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
                return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name}", new
                {
                    T.Name,
                    T.Creator,
                    AddRoles = T.Filds.Where(f =>
                    {
                        return
                            (f?.IsPublic ?? false) ||
                            (f?.AddRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
                            (((f?.IfOneRole ?? false) && T.Filds.Any(p => p.AddRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))));
                    }),
                    ReadRoles = T.Filds.Where(f =>
                    {
                        return
                            (f?.IsPublic ?? false) ||
                            (f?.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
                            (((f?.IfOneRole ?? false) && T.Filds.Any(p => p.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))));
                    }),
                    DeleteRoles = T.Filds.Where(f =>
                    {
                        return
                            (f?.IsPublic ?? false) ||
                            (f?.DeleteRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
                            (((f?.IfOneRole ?? false) && T.Filds.Any(p => p.DeleteRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))));
                    }),
                    UpdateRoles = T.Filds.Where(f =>
                    {
                        return
                            (f?.IsPublic ?? false) ||
                            (f?.UpdateRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
                            (((f?.IfOneRole ?? false) && T.Filds.Any(p => p.UpdateRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))));
                    }),
                });
            else
                return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        }
        public static Return TablesList(DBWUser UM, DBWTabels TM, HttpContext http)
        {
            var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
            return Return.Sucsses(Return.Loc.Endpoint, $"TableS",
               TM.GetAll().ToList());
            //   .Select(T =>
            //      (T.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName))) ? new
            //      {
            //          T.Name,
            //          T.Creator,
            //          AddRoles = T.Filds.Where(f =>
            //          {
            //              return
            //                  (f?.IsPublic ?? false) ||
            //                  (f?.AddRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
            //                  (((f?.IfOneRole ?? false) && T.Filds.Any(p => p.AddRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))));
            //          }),
            //          ReadRoles = T.Filds.Where(f =>
            //          {
            //              return
            //                  (f?.IsPublic ?? false) ||
            //                  (f?.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
            //                  (((f?.IfOneRole ?? false) && T.Filds.Any(p => p.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))));
            //          }),
            //          DeleteRoles = T.Filds.Where(f =>
            //          {
            //              return
            //                  (f?.IsPublic ?? false) ||
            //                  (f?.DeleteRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
            //                  (((f?.IfOneRole ?? false) && T.Filds.Any(p => p.DeleteRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))));
            //          }),
            //          UpdateRoles = T.Filds.Where(f =>
            //          {
            //              return
            //                  (f?.IsPublic ?? false) ||
            //                  (f?.UpdateRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
            //                  (((f?.IfOneRole ?? false) && T.Filds.Any(p => p.UpdateRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))));
            //          }),
            //      } : null
            //    ).ToList()
            //);
        }
        public static Return TablesRemove(string Name, DBWUser UM, DBWTabels TM, HttpContext http)
        {
            var T = TM.First(t => t.Name == Name);
            if (T == null)
                return Return.ThisExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
            var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
            {
                if (T.Creator.Name == User!.UserName)
                    goto DoDeleteTable;
                var P = T.Creator.Parent;
                while (true)
                {
                    if (P == null)
                        break;
                    if (P.UserName == User.UserName)
                        goto DoDeleteTable;
                    P = P.Parent;
                }
            }
            return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Parent");
        DoDeleteTable:
            {
                TM.Remove(T, User);
                return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Deleted");
            }
        }
        public static Return TablesAddField(string Name, Database.Table.SQL.FieldsTable Field, DBWUser UM, DBWTabels TM, HttpContext http)
        {
            var T = TM.First(t => t.Name == Name);

            if (T == null)
                return Return.ThisExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
            var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
            {
                if (T.Creator.Name == User!.UserName)
                    goto DoAddField;
                if (T.Editors.Any(p => p.UserName == User.UserName))
                    goto DoAddField;
                var P = T.Creator.Parent;
                while (true)
                {
                    if (P == null)
                        break;
                    if (P.UserName == User.UserName)
                        goto DoAddField;
                    P = P.Parent;
                }
            }
            return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Parent Or Editor");
        DoAddField:
            {
                TM.AddField(Field, T);
                return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Field {Field} Added");
            }
        }
        public static Return TablesRemoveField(string Name, string Field, DBWUser UM, DBWTabels TM, HttpContext http)
        {
            var T = TM.First(t => t.Name == Name);
            if (T == null)
                return Return.ThisExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
            var F = TM.FirstField(Name, Field);
            if (F == null)
                return Return.ThisExist(Return.Loc.Endpoint, $"Field {Name} Not Exist");
            var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
            {
                if (T.Creator.Name == User!.UserName)
                    goto DoRemoveField;
                if (T.Editors.Any(p => p.UserName == User.UserName))
                    goto DoRemoveField;
                var P = T.Creator.Parent;
                while (true)
                {
                    if (P == null)
                        break;
                    if (P.UserName == User.UserName)
                        goto DoRemoveField;
                    P = P.Parent;
                }
            }
            return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Parent Or Editor");
        DoRemoveField:
            {
                TM.RemoveField(F, T);
                return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Field {Field} Removed");
            }
        }
        public static Return TablesUpdateField(string Name, Database.Table.SQL.FieldsTable Field, DBWUser UM, DBWTabels TM, HttpContext http)
        {
            var T = TM.First(t => t.Name == Name);
            if (T == null)
                return Return.ThisExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
            var F = TM.FirstField(Name, Field.Name);
            if (F == null)
                return Return.ThisExist(Return.Loc.Endpoint, $"Field {Name} Not Exist");
            var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
            {
                if (T.Creator.Name == User!.UserName)
                    goto DoUpdateField;
                if (T.Editors.Any(p => p.UserName == User.UserName))
                    goto DoUpdateField;
                var P = T.Creator.Parent;
                while (true)
                {
                    if (P == null)
                        break;
                    if (P.UserName == User.UserName)
                        goto DoUpdateField;
                    P = P.Parent;
                }
            }
            return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Parent Or Editor");
        DoUpdateField:
            {
                TM.UpdateField(Field);
                return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Field {Field} Updated");
            }
        }
        public static Return TablesGetField(string Name, string Field, DBWUser UM, DBWTabels TM, HttpContext http)
        {
            var T = TM.First(t => t.Name == Name);
            if (T == null)
                return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
            var F = TM.FirstField(Name, Field);
            if (F == null)
                return Return.NotExist(Return.Loc.Endpoint, $"Field {Name} Not Exist");
            var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
            if (F.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
                return Return.Sucsses(Return.Loc.Endpoint, $"Field Found", new { F.Name, F.Auto, F.Value, F.Type, F.Disable, F.IfOneRole, F.IsPublic, F.IsUnique, F.Null, F.Show });
            return Return.AccessDeny(Return.Loc.Endpoint, $"Field {Name} Not Exist");
        }

    }
}
