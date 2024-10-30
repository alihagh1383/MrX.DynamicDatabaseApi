namespace MrX.DynamicDatabaseApi.Database.Table.SQL;

public class RolesTable : BaseTable
{
    public required string Name { get; set; }  
    public required UsersTable Creator { get; set; }
    public List<FieldsTable> FieldsAddRole { get; set; } = [];
    public List<FieldsTable> FieldsDeleteRole { get; set; } = [];
    public List<FieldsTable> FieldsUpdateRole { get; set; } = [];
    public List<FieldsTable> FieldsReadRole { get; set; } = [];
    public List<TablesTable> TabelsAddRole { get; set; } = [];
    public List<TablesTable> TabelsDeleteRole { get; set; } = [];
    public List<TablesTable> TabelsUpdateRole { get; set; } = [];
    public List<TablesTable> TabelsReadRole { get; set; } = [];
    public List<string> PathsRole { get; set; } = [];
    public List<UsersTable> Users { get; set; } = [];
}