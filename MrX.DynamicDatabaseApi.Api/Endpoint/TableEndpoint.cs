using MrX.DynamicDatabaseApi.CallBack;
using MrX.DynamicDatabaseApi.Database.Table.Dynamic;
using MrX.DynamicDatabaseApi.Worker;

namespace MrX.DynamicDatabaseApi.Api.Endpoint;

public class TableEndpoint
{
    public static Return TableAdd(string Name, DBWUser UM, DBWTabels TM, HttpContext http)
    {
        var T = TM.First(p => p.Name == Name);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        if (!T.AddRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
            return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Add To Table {Name}");
        var TE = new DynamicTable();
        var DM = new DBWDynamicTable(Name);
        DM.Add(TE);
        return Return.Sucsses(Return.Loc.Endpoint, "Row Created", TE);
    }

    public static Return TableRemove(string Name, Guid Row, DBWUser UM, DBWTabels TM, HttpContext http)
    {
        var T = TM.First(p => p.Name == Name);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        if (!T.DeleteRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
            return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Remove From Table {Name}");
        var DM = new DBWDynamicTable(Name);
        var R = DM.Find(Row);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Row {Row} Not Exist");
        DM.Remove(R);
        return Return.Sucsses(Return.Loc.Endpoint, "Row Deleted", Row);
    }

    public static Return TableGet(string Name, Guid Row, DBWUser UM, DBWTabels TM, HttpContext http)
    {
        var T = TM.First(p => p.Name == Name);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        if (!T.DeleteRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
            return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Read From Table {Name}");
        var DM = new DBWDynamicTable(Name);
        var R = DM.Find(Row);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Row {Row} Not Exist");
        return Return.Sucsses(Return.Loc.Endpoint, "Row Deleted", new
        {
            R.CreatedDate,
            R.Id,
            Properties = R.DynamicColumns.Where(p =>
            {
                var f = T.Filds.Find(p2 => p2.Name == p.Key);
                return
                    (f?.IsPublic ?? false) ||
                    (f?.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
                    ((f?.IfOneRole ?? false) && T.Filds.Any(p =>
                        p.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName))));
            }).ToDictionary()
        });
    }

    public static Return TableList(string Name, DBWUser UM, DBWTabels TM, HttpContext http)
    {
        var T = TM.First(p => p.Name == Name);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        if (!T.DeleteRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
            return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Read From Table {Name}");
        var DM = new DBWDynamicTable(Name);

        return Return.Sucsses(Return.Loc.Endpoint, "Row Deleted", DM.GetAll().Select(R => new
        {
            R.CreatedDate,
            R.Id,
            Properties = R.DynamicColumns.Where(p =>
            {
                var f = T.Filds.Find(p2 => p2.Name == p.Key);
                return
                    (f?.IsPublic ?? false) ||
                    (f?.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)) ?? false) ||
                    ((f?.IfOneRole ?? false) && T.Filds.Any(p =>
                        p.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName))));
            }).ToDictionary()
        }));
    }

    public static Return TableAddField(string Name, Guid Row, string FieldName, string Value, DBWUser UM, DBWTabels TM,
        HttpContext http)
    {
        var T = TM.First(p => p.Name == Name);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var F = T.Filds.First(p => p.Name == FieldName);
        if (F == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} Not Exist");
        if (!F.AddRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
            return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Add To Table {Name} Field {FieldName}");
        var DM = new DBWDynamicTable(Name);
        var R = DM.First(p => p.Id == Row);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Row Not Exist");
        if (R.DynamicColumns.ContainsKey(FieldName))
            return Return.Invalid(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} In This Row Is Added");
        R.DynamicColumns.Add(FieldName, F.Type + ":" + Value);
        DM.Update(R);
        return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} In This Row Is Add");
    }

    public static Return TableUpdateField(string Name, Guid Row, string FieldName, string Value, DBWUser UM,
        DBWTabels TM, HttpContext http)
    {
        var T = TM.First(p => p.Name == Name);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var F = T.Filds.First(p => p.Name == FieldName);
        if (F == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} Not Exist");
        if (!F.UpdateRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
            return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Add To Table {Name} Field {FieldName}");
        var DM = new DBWDynamicTable(Name);
        var R = DM.First(p => p.Id == Row);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Row Not Exist");
        R.DynamicColumns[FieldName] = F.Type + ":" + Value;
        DM.Update(R);
        return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} In This Row Is Add");
    }

    public static Return TableRemoveField(string Name, Guid Row, string FieldName, DBWUser UM, DBWTabels TM,
        HttpContext http)
    {
        var T = TM.First(p => p.Name == Name);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var F = T.Filds.First(p => p.Name == FieldName);
        if (F == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} Not Exist");
        if (!F.DeleteRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
            return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Remove From Table {Name} Field {FieldName}");
        var DM = new DBWDynamicTable(Name);
        var R = DM.First(p => p.Id == Row);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Row Not Exist");
        if (!R.DynamicColumns.ContainsKey(FieldName))
            return Return.Invalid(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} In This Row Is Null");
        R.DynamicColumns.Remove(FieldName);
        DM.Update(R);
        return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} In This Row Is Removed");
    }

    public static Return TableGetField(string Name, Guid Row, string FieldName, DBWUser UM, DBWTabels TM,
        HttpContext http)
    {
        var T = TM.First(p => p.Name == Name);
        if (T == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Not Exist");
        var User = UM.Find(p => p.UserName == http.User.Identity!.Name);
        var F = T.Filds.First(p => p.Name == FieldName);
        if (F == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} Not Exist");
        if (!F.ReadRoles.Any(p => p.Users.Any(p => p.UserName == User!.UserName)))
            return Return.AccessDeny(Return.Loc.Endpoint, $"You Can Not Remove From Table {Name} Field {FieldName}");
        var DM = new DBWDynamicTable(Name);
        var R = DM.First(p => p.Id == Row);
        if (R == null)
            return Return.NotExist(Return.Loc.Endpoint, $"Table {Name} Row Not Exist");
        if (!R.DynamicColumns.ContainsKey(FieldName))
            return Return.Invalid(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} In This Row Is Null");
        return Return.Sucsses(Return.Loc.Endpoint, $"Table {Name} Field {FieldName} In This Row",
            R.DynamicColumns[FieldName]);
    }
}