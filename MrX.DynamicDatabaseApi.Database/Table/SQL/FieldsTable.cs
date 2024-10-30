namespace MrX.DynamicDatabaseApi.Database.Table.SQL;

public class FieldsTable : BaseTable
{
    public string Name                  { get; set; } = string.Empty;
    public string Value                 { get; set; } = string.Empty;
    public string Type                  { get; set; } = string.Empty;
    public bool Auto                    { get; set; } = false;
    public bool Show                    { get; set; } = true;
    public bool Disable                 { get; set; } = false;
    public bool Null                    { get; set; } = true;
    public bool IsPublic                { get; set; } = false;
    public bool IsUnique                { get; set; } = false;
    public bool IfOneRole               { get; set; } = false;
    public required TablesTable Table   { get; set; }
    public List<RolesTable> AddRoles    { get; set; } = new List<RolesTable>();
    public List<RolesTable> DeleteRoles { get; set; } = new List<RolesTable>();
    public List<RolesTable> UpdateRoles { get; set; } = new List<RolesTable>();
    public List<RolesTable> ReadRoles   { get; set; } = new List<RolesTable>();

}