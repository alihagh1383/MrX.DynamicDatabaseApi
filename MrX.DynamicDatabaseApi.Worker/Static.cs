using MrX.DynamicDatabaseApi.Database;

namespace MrX.DynamicDatabaseApi.Worker;

public static class Static
{
    public static readonly Dictionary<string, DynamicDbContext> Ddbcs = new();
    public static string? ConStr { get; set; }
}