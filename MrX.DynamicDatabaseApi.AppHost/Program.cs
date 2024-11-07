var builder = DistributedApplication.CreateBuilder(args);
var DB = builder.AddMySql("MySQL").AddDatabase("DB");
builder.AddProject<Projects.MrX_DynamicDatabaseApi_Api>("mrx-dynamicdatabaseapi-api")
    .WithReference(DB)
    .WithExternalHttpEndpoints()
    ;

builder.Build().Run();
