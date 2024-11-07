
var builder = DistributedApplication.CreateBuilder(args);
var DB = builder.AddMySql("MySQL").AddDatabase("DB");
var SEQ = builder.AddSeq("SQE")
    //.WithDataVolume()
    .WithDataBindMount("seqdata")
;
builder.AddProject<Projects.MrX_DynamicDatabaseApi_Api>("mrx-dynamicdatabaseapi-api")
    .WithReference(DB)
    .WithReference(SEQ)
    .WithExternalHttpEndpoints()
    ;

builder.Build().Run();
