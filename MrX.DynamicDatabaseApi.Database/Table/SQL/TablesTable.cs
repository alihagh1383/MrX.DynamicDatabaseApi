namespace MrX.DynamicDatabaseApi.Database.Table.SQL;

public class TablesTable : BaseTable
{
    public string Name { get; set; } = string.Empty;
    public required UsersTable Creator { get; set; }
    public List<UsersTable> Editors { get; set; } = [];
    public List<FieldsTable> Filds { get; set; } = [];
    public List<RolesTable> AddRoles { get; set; } = [];
    public List<RolesTable> DeleteRoles { get; set; } = [];
    public List<RolesTable> UpdateRoles { get; set; } = [];
    public List<RolesTable> ReadRoles { get; set; } = [];

}