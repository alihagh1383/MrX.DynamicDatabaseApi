using System.ComponentModel.DataAnnotations;

namespace MrX.DynamicDatabaseApi.Database.Table;

public class BaseTable
{
    [Key] public Guid Id { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public Guid LastUpdate { get; set; }
}