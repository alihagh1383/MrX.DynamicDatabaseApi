using Newtonsoft.Json;

namespace MrX.DynamicDatabaseApi.Database.Table.InMemory;

public class LoginsTables : BaseTable
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTimeOffset Expire { get; set; } = DateTimeOffset.Now.AddDays(1);
    public Guid? UserLastUpdate { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public bool IsThisSession(string name, string password)
    {
        return Username == name.ToLower() && password == Password;
    }
}