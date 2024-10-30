var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.MrX_DynamicDatabaseApi_Api>("API");
builder.Build().Run();