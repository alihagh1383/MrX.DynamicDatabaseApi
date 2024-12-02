using MrX.DynamicDatabaseApi.CallBack;
using MrX.DynamicDatabaseApi.Database.Table.SQL;
using MrX.DynamicDatabaseApi.Worker;

namespace MrX.DynamicDatabaseApi.Api.Endpoint;

public class RoleEndpoint
{
    public static Return RoleGet(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);

        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        if (User!.Roles.Any(p => p.Name == RoleName))
            return Return.Sucsses(Return.Loc.Endpoint, $"Role {RoleName}", R);

        foreach (var u in User!.Childs)
            if (ExistRole(u))
                return Return.Sucsses(Return.Loc.Endpoint, $"Role {RoleName}", R);

        bool ExistRole(UsersTable user)
        {
            if (user.Roles.Any(p => p.Name == RoleName))
                return true;
            foreach (var u in user.Childs)
                if (ExistRole(u))
                    return true;
            return false;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not See Role {RoleName}");
    }

    public static Return RoleList(DBWUser UM, DBWRoles RM, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var r = User!.Roles.ToList();
        foreach (var u in User!.Childs)
            GetRole(u);

        void GetRole(UsersTable user)
        {
            r.AddRange(user.Roles);
            foreach (var u in user.Childs)
                GetRole(u);
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Redeble Roles", r);
    }

    public static Return RoleAdd(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        if (RM.Exist(p => p.Name == RoleName))
            return Return.ThisExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Exist");
        var Role = new RolesTable { Creator = User!, Name = RoleName };
        Role.Users.Add(User!);
        RM.Add(Role);
        return Return.Sucsses(Return.Loc.Endpoint, $"Role {RoleName} Create");
    }

    public static Return RoleRemove(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        RM.Remove(R);
        return Return.Sucsses(Return.Loc.Endpoint, $"Role {RoleName} Create");
    }

    public static Return RoleAddField(DBWUser UM, DBWRoles RM, DBWTabels TM, HttpContext http, string RoleName,
        string Table, string Field, string Mode)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var T = TM.First(p => p.Name == Table);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table By Name {Table} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        {
            switch (Mode.ToLower())
            {
                case "add":
                {
                    var F = User.Roles.Select(p => p.FieldsAddRole.First(p => p.Name == Field && p.Table.Name == Table))
                        .First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}.{Field}");
                    if (!R.FieldsAddRole.Contains(F)) R.FieldsAddRole.Add(F);
                    if (!R.TabelsAddRole.Contains(T)) R.TabelsAddRole.Add(T);
                    if (!R.FieldsReadRole.Contains(F)) R.FieldsReadRole.Add(F);
                    if (!R.TabelsReadRole.Contains(T)) R.TabelsReadRole.Add(T);
                    break;
                }
                case "delete":
                {
                    var F = User.Roles
                        .Select(p => p.FieldsDeleteRole.First(p => p.Name == Field && p.Table.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}.{Field}");
                    if (!R.FieldsDeleteRole.Contains(F)) R.FieldsDeleteRole.Add(F);
                    if (!R.TabelsDeleteRole.Contains(T)) R.TabelsDeleteRole.Add(T);
                    if (!R.FieldsReadRole.Contains(F)) R.FieldsReadRole.Add(F);
                    if (!R.TabelsReadRole.Contains(T)) R.TabelsReadRole.Add(T);
                    break;
                }
                case "read":
                {
                    var F = User.Roles
                        .Select(p => p.FieldsReadRole.First(p => p.Name == Field && p.Table.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}.{Field}");

                    if (!R.FieldsReadRole.Contains(F)) R.FieldsReadRole.Add(F);
                    if (!R.TabelsReadRole.Contains(T)) R.TabelsReadRole.Add(T);
                    break;
                }
                case "update":
                {
                    var F = User.Roles
                        .Select(p => p.FieldsUpdateRole.First(p => p.Name == Field && p.Table.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}.{Field}");
                    if (!R.FieldsAddRole.Contains(F)) R.FieldsAddRole.Add(F);
                    if (!R.TabelsAddRole.Contains(T)) R.TabelsAddRole.Add(T);
                    if (!R.FieldsDeleteRole.Contains(F)) R.FieldsDeleteRole.Add(F);
                    if (!R.TabelsDeleteRole.Contains(T)) R.TabelsDeleteRole.Add(T);
                    if (!R.FieldsReadRole.Contains(F)) R.FieldsReadRole.Add(F);
                    if (!R.TabelsReadRole.Contains(T)) R.TabelsReadRole.Add(T);
                    if (!R.FieldsUpdateRole.Contains(F)) R.FieldsUpdateRole.Add(F);
                    if (!R.TabelsUpdateRole.Contains(T)) R.TabelsUpdateRole.Add(T);
                    break;
                }
                default:
                    return Return.Invalid(Return.Loc.Endpoint, $"Mode {Mode} Is Invalid");
            }

            RM.Update(R);
            return Return.Sucsses(Return.Loc.Endpoint,
                $"Field {Field} In Table {Table} Add To Role {R.Name} By Mode {Mode}");
        }
    }

    public static Return RoleRemoveField(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName, string Table,
        string Field, string Mode)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        {
            switch (Mode.ToLower())
            {
                case "add":
                {
                    var F = User.Roles.Select(p => p.FieldsAddRole.First(p => p.Name == Field && p.Table.Name == Table))
                        .First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}.{Field}");
                    if (R.FieldsAddRole.Contains(F)) R.FieldsAddRole.Remove(F);
                    if (R.FieldsUpdateRole.Contains(F)) R.FieldsUpdateRole.Remove(F);
                    break;
                }
                case "delete":
                {
                    var F = User.Roles
                        .Select(p => p.FieldsDeleteRole.First(p => p.Name == Field && p.Table.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}.{Field}");
                    if (R.FieldsDeleteRole.Contains(F)) R.FieldsDeleteRole.Remove(F);
                    if (R.FieldsUpdateRole.Contains(F)) R.FieldsUpdateRole.Remove(F);
                    break;
                }
                case "read":
                {
                    var F = User.Roles
                        .Select(p => p.FieldsReadRole.First(p => p.Name == Field && p.Table.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}.{Field}");
                    if (R.FieldsReadRole.Contains(F)) R.FieldsReadRole.Remove(F);
                    if (R.FieldsAddRole.Contains(F)) R.FieldsAddRole.Remove(F);
                    if (R.FieldsDeleteRole.Contains(F)) R.FieldsDeleteRole.Remove(F);
                    if (R.FieldsUpdateRole.Contains(F)) R.FieldsUpdateRole.Remove(F);
                    break;
                }
                case "update":
                {
                    var F = User.Roles
                        .Select(p => p.FieldsUpdateRole.First(p => p.Name == Field && p.Table.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}.{Field}");
                    if (R.FieldsUpdateRole.Contains(F)) R.FieldsUpdateRole.Remove(F);
                    break;
                }
                default:
                    return Return.Invalid(Return.Loc.Endpoint, $"Mode {Mode} Is Invalid");
            }

            RM.Update(R);
            return Return.Sucsses(Return.Loc.Endpoint,
                $"Field {Field} In Table {Table} Remove From Role {R.Name} By Mode {Mode}");
        }
    }

    public static Return RoleAddTable(DBWUser UM, DBWTabels TM, DBWRoles RM, HttpContext http, string RoleName,
        string Table, string Mode)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var T = TM.First(p => p.Name == Table);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table By Name {Table} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        {
            switch (Mode.ToLower())
            {
                case "add":
                {
                    var F = User.Roles.Select(p => p.TabelsAddRole.First(p => p.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}");
                    if (!R.TabelsAddRole.Contains(T)) R.TabelsAddRole.Add(T);
                    if (!R.TabelsReadRole.Contains(T)) R.TabelsReadRole.Add(T);
                    break;
                }
                case "delete":
                {
                    var F = User.Roles.Select(p => p.TabelsDeleteRole.First(p => p.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}");
                    if (!R.TabelsDeleteRole.Contains(T)) R.TabelsDeleteRole.Add(T);
                    if (!R.TabelsReadRole.Contains(T)) R.TabelsReadRole.Add(T);
                    break;
                }
                case "read":
                {
                    var F = User.Roles.Select(p => p.TabelsReadRole.First(p => p.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}");

                    if (!R.TabelsReadRole.Contains(T)) R.TabelsReadRole.Add(T);
                    break;
                }
                case "update":
                {
                    var F = User.Roles.Select(p => p.TabelsUpdateRole.First(p => p.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}");
                    if (!R.TabelsAddRole.Contains(T)) R.TabelsAddRole.Add(T);
                    if (!R.TabelsDeleteRole.Contains(T)) R.TabelsDeleteRole.Add(T);
                    if (!R.TabelsReadRole.Contains(T)) R.TabelsReadRole.Add(T);
                    if (!R.TabelsUpdateRole.Contains(T)) R.TabelsUpdateRole.Add(T);
                    break;
                }
                default:
                    return Return.Invalid(Return.Loc.Endpoint, $"Mode {Mode} Is Invalid");
            }

            RM.Update(R);
            return Return.Sucsses(Return.Loc.Endpoint, $"Table {Table} Add To Role {R.Name} By Mode {Mode}");
        }
    }

    public static Return RoleRemoveTable(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName, string Table,
        string Mode)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        {
            switch (Mode.ToLower())
            {
                case "add":
                {
                    var F = User.Roles.Select(p => p.TabelsAddRole.First(p => p.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}");
                    if (R.TabelsAddRole.Contains(F)) R.TabelsAddRole.Remove(F);
                    if (R.TabelsUpdateRole.Contains(F)) R.TabelsUpdateRole.Remove(F);
                    break;
                }
                case "delete":
                {
                    var F = User.Roles.Select(p => p.TabelsDeleteRole.First(p => p.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}");
                    if (R.TabelsDeleteRole.Contains(F)) R.TabelsDeleteRole.Remove(F);
                    if (R.TabelsUpdateRole.Contains(F)) R.TabelsUpdateRole.Remove(F);
                    break;
                }
                case "read":
                {
                    var F = User.Roles.Select(p => p.TabelsReadRole.First(p => p.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}");
                    if (R.TabelsReadRole.Contains(F)) R.TabelsReadRole.Remove(F);
                    if (R.TabelsAddRole.Contains(F)) R.TabelsAddRole.Remove(F);
                    if (R.TabelsDeleteRole.Contains(F)) R.TabelsDeleteRole.Remove(F);
                    if (R.TabelsUpdateRole.Contains(F)) R.TabelsUpdateRole.Remove(F);
                    break;
                }
                case "update":
                {
                    var F = User.Roles.Select(p => p.TabelsUpdateRole.First(p => p.Name == Table)).First();
                    if (F == null)
                        return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {Table}");
                    if (R.TabelsUpdateRole.Contains(F)) R.TabelsUpdateRole.Remove(F);
                    break;
                }
                default:
                    return Return.Invalid(Return.Loc.Endpoint, $"Mode {Mode} Is Invalid");
            }

            RM.Update(R);
            return Return.Sucsses(Return.Loc.Endpoint, $" Table {Table} Remove From Role {R.Name} By Mode {Mode}");
        }
    }

    public static Return RoleAddUser(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName, string UserName)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        {
            var U = UM.First(p => p.UserName == UserName);
            if (U == null)
                return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {UserName} Not Exist");

            var P = U.Parent;
            while (true)
            {
                if (P == null)
                    break;
                if (P.UserName == User!.UserName)
                    goto DO;
                P = P.Parent;
            }

            return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Parent");
            DO:
            {
                if (!R.Users.Contains(U))
                    R.Users.Add(U);
            }
            return Return.Sucsses(Return.Loc.Endpoint, $"User {UserName} Added");
        }
    }

    public static Return RoleRemoveUser(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName, string UserName)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        {
            var U = UM.First(p => p.UserName == UserName);
            if (U == null)
                return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {UserName} Not Exist");

            var P = U.Parent;
            while (true)
            {
                if (P == null)
                    break;
                if (P.UserName == User!.UserName)
                    goto DO;
                P = P.Parent;
            }

            return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Parent");
            DO:
            {
                if (R.Users.Contains(U))
                    R.Users.Remove(U);
            }
            return Return.Sucsses(Return.Loc.Endpoint, $"User {UserName} Removed");
        }
    }

    public static Return RoleAddPath(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName, string path)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        {
            var F = User.Roles.Select(p => p.PathsRole.First(p => p == path)).First();
            if (F == null)
                return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {path}");
            if (!R.PathsRole.Contains(F)) R.PathsRole.Add(path);
            return Return.Sucsses(Return.Loc.Endpoint, $"Path {path} Added");
        }
    }

    public static Return RoleRemovePath(DBWUser UM, DBWRoles RM, HttpContext http, string RoleName, string path)
    {
        if (RoleName.StartsWith('_'))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {_}");
        if (RoleName.StartsWith("Table"))
            return Return.Invalid(Return.Loc.Endpoint, "Role Name Can Not Start With {Table}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var R = RM.First(p => p.Name == RoleName);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");
        var C = R.Creator;
        while (true)
        {
            if (C.UserName == User!.UserName)
                goto Do;
            C = C.Parent;
            if (C == null)
                break;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not Creator Or Patent");
        Do:
        {
            var F = User.Roles.Select(p => p.PathsRole.First(p => p == path)).First();
            if (F == null)
                return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Cheange Role {path}");
            if (R.PathsRole.Contains(F)) R.PathsRole.Remove(path);
            return Return.Sucsses(Return.Loc.Endpoint, $"Path {path} Removed");
        }
    }
}