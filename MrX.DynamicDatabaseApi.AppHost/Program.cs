var builder = DistributedApplication.CreateBuilder(options: new DistributedApplicationOptions() { DisableDashboard = false });
var SQL = builder.AddMySql("SQL"    );
var DB = SQL.AddDatabase("DB", "DB");
var DBApi = builder.AddProject<Projects.MrX_DynamicDatabaseApi_Api>("DBAPI").WithExternalHttpEndpoints();

DBApi.WithReference(DB);
//DBApi.WaitFor(DB);
builder.Build().Run();