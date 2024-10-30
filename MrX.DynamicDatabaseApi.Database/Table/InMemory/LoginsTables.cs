namespace MrX.DynamicDatabaseApi.Database.Table.InMemory;

public class LoginsTables :BaseTable
{
    public string Username            { get; set; }         = string.Empty;
    public string Password            { get; set; }            = string.Empty;
    public DateTimeOffset Expire     { get; set; }            = DateTimeOffset.Now.AddDays(1);
    public override string ToString()
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
    public bool IsThisSession(string name, string password) => (Username == name.ToLower() && (password == Password));

}