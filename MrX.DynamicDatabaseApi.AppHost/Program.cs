var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MrX_DynamicDatabaseApi_ApiService>("apiservice");

builder.Build().Run();
