using MrX.DynamicDatabaseApi.CallBack;
using MrX.DynamicDatabaseApi.Database.Table.SQL;
using MrX.DynamicDatabaseApi.Worker;
using MrX.Security;

namespace MrX.DynamicDatabaseApi.Api.Endpoint;

public class UserEndpoint
{
    public static Return UserAdd(DBWUser UM, string UserName, string Password, HttpContext http)
    {
        if (UM.Exist(p => p.UserName == UserName))
            return Return.ThisExist(Return.Loc.Endpoint, $"User By UserName {UserName} Exist");
        if (string.IsNullOrWhiteSpace(Password))
            return Return.Invalid(Return.Loc.Endpoint, $"Password Not True {Password}");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        UM.Add(new UsersTable
        {
            Parent = User,
            Password = PasswordHash.Hash(Password),
            UserName = UserName
        });
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName} Create");
    }

    public static Return UserGet(DBWUser UM, string UserName, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var U = UM.First(p => p.UserName == UserName);
        if (U == null)
            return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {UserName} Not Exist");
        if (U.UserName == User!.UserName)
            goto DO;
        var P = U.Parent;
        while (true)
        {
            if (P == null)
                break;
            if (P.UserName == User.UserName)
                goto DO;
            P = P.Parent;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not User Or Creator Or Parent");
        DO:
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName}",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserMy(DBWUser UM, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var U = User!;
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {U.UserName}",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserList(DBWUser UM, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var c = User!.Childs;
        c.AddRange(User.Childs.SelectMany(p => GetChilds(p)));
        return Return.Sucsses(Return.Loc.Endpoint, "Users",
            c.Select(p => new
                { p.UserName, p.EmailAddress, p.Id, p.CreatedDate, p.Name, p.Parent, p.PhoneNumber, p.Roles }));

        List<UsersTable> GetChilds(UsersTable Userc)
        {
            List<UsersTable> T = [];
            if (Userc.Childs.Count == 0)
                T.Add(User);
            else
                T.AddRange(Userc.Childs.SelectMany(p => GetChilds(p)));
            return T;
        }
    }

    public static Return UserRemove(DBWUser UM, string UserName, HttpContext http)
    {
        var U = UM.First(p => p.UserName == UserName);
        if (U == null)
            return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {UserName} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var P = U!.Parent;
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
        UM.Remove(U);
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName} Is Remove",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserUpdate(DBWUser UM, string UserName, string Name, string Phone, string Email,
        HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
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
        U.PhoneNumber = Phone;
        U.EmailAddress = Email;
        U.Name = Name;
        UM.Update(U);
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName} Updated",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserAddData(DBWUser UM, string UserName, string Name, string Data, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
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
        U.Data.Add(Name, Data);
        UM.Update(U);
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName} Updated",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserAddRole(DBWUser UM, DBWRoles RM, string UserName, string RoleName, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var U = UM.First(p => p.UserName == UserName);
        var R = RM.First(p => p.Name == RoleName);
        if (U == null)
            return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {UserName} Not Exist");
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");

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
        U.Roles.Add(R);
        UM.Update(U);
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName} Updated",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserRemoveData(DBWUser UM, string UserName, string Name, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
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
        U.Data.Remove(Name);
        UM.Update(U);
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName} Updated",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserRemoveRole(DBWUser UM, DBWRoles RM, string UserName, string RoleName, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var U = UM.First(p => p.UserName == UserName);
        var R = RM.First(p => p.Name == RoleName);
        if (U == null)
            return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {UserName} Not Exist");
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Role By Name {RoleName} Not Exist");

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
        U.Roles.Remove(R);
        UM.Update(U);
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName} Updated",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserPublicList(DBWUser UM, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        return Return.Sucsses(Return.Loc.Endpoint, "Users",
            UM.GetAll().Select(p => new
            {
                p.Name,
                p.Parent,
                p.Childs,
                p.CreatedDate
            }));
    }

    public static Return UserCheangeParent(DBWUser UM, string UserName, string ParentName, HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var U = UM.First(p => p.UserName == UserName);
        var NP = UM.First(p => p.Name == ParentName);
        if (U == null)
            return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {UserName} Not Exist");
        if (NP == null)
            return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {ParentName} Not Exist");
        var P = U.Parent;
        while (true)
        {
            if (P == null)
                break;
            if (P.UserName == User!.UserName)
                goto DNEXT;
            P = P.Parent;
        }

        DNEXT:
        P = NP.Parent;
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
        U.Parent = NP;
        UM.Update(U);
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName} Updated",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }

    public static Return UserUpdatePassword(DBWUser UM, string UserName, string NewPassword, string? password,
        HttpContext http)
    {
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var U = UM.First(p => p.UserName == UserName);
        if (U == null)
            return Return.NotExist(Return.Loc.Endpoint, $"User By UserName {UserName} Not Exist");
        if (U.UserName == User!.UserName && U.CheckPassword(password ?? ""))
            goto DO;
        var P = U.Parent;
        while (true)
        {
            if (P == null)
                break;
            if (P.UserName == User.UserName)
                goto DO;
            P = P.Parent;
        }

        return Return.AccessDeny(Return.Loc.Endpoint, "You Are Not User Or Creator Or Parent");
        DO:
        U.Password = PasswordHash.Hash(NewPassword);
        UM.Update(U);
        return Return.Sucsses(Return.Loc.Endpoint, $"User By Name {UserName}",
            new { U.Name, U.Roles, U.UserName, U.PhoneNumber, U.EmailAddress, U.Id, U.Data });
    }
}