namespace MrX.DynamicDatabaseApi.Database.Table.Dynamic;

public class DynamicTable : BaseTable
{
   public Dictionary<string,string> DynamicColumns { get; set; } = new();
}