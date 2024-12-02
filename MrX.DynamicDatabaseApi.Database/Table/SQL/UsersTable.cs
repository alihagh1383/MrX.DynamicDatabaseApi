using MrX.Security;

namespace MrX.DynamicDatabaseApi.Database.Table.SQL;

public class UsersTable : BaseTable
{
    public string Name { get; set; } = string.Empty;
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public List<RolesTable> Roles { get; set; } = [];
    public List<RolesTable> CreateRoles { get; set; } = [];
    public Dictionary<string, object> Data { get; set; } = [];
    public UsersTable? Parent { get; set; }
    public List<UsersTable> Childs { get; set; } = [];

    public bool CheckPassword(string password)
    {
        return PasswordHash.Verify(password, Password);
    }

    public bool CheckIsThisUser(string usernameOrEmail, string password)
    {
        return (UserName.Equals(usernameOrEmail, StringComparison.CurrentCultureIgnoreCase) ||
                EmailAddress.Equals(usernameOrEmail, StringComparison.CurrentCultureIgnoreCase)) &&
               CheckPassword(password);
    }
}