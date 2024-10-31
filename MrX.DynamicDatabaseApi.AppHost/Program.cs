var builder = DistributedApplication.CreateBuilder(args);
var SQL = builder.AddSqlServer("SQL");
var DB = SQL.AddDatabase("DB", "DB");
var Cache = builder.AddRedis("Cache");
var DBApi = builder.AddProject<Projects.MrX_DynamicDatabaseApi_Api>("DBAPI");
DBApi.WithReference(Cache);
DBApi.WithReference(DB);
DBApi.WaitFor(DB);
builder.Build().Run();