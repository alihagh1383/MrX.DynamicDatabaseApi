
var builder = DistributedApplication.CreateBuilder(args);
var DB = builder.AddMySql("MySQL").AddDatabase("DB");
var SEQ = builder.AddSeq("SQE")
    .WithDataBindMount("seqdata")
;
builder.AddProject<Projects.MrX_DynamicDatabaseApi_Api>("mrx-dynamicdatabaseapi-api")
    .WithReference(DB)
    .WithReference(SEQ)
    .WithHttpEndpoint(8000)
    .WithExternalHttpEndpoints()
    ;

builder.Build().Run();
