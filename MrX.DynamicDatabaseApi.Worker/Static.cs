namespace MrX.DynamicDatabaseApi.Worker { 

public static partial class Static
{
    public static readonly Dictionary<string, MrX.DynamicDatabaseApi.Database.DynamicDbContext> Ddbcs = new();
    public static string? ConStr { get; set; }

}}