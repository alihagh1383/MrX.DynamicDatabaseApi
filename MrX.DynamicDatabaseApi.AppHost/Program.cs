var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions() { AllowUnsecuredTransport = true });

var SQL = builder.AddMySql("SQL", port: 8000).PublishAsContainer();
var DB = SQL.AddDatabase("DB");
var SEQ = builder.AddSeq("SEQ", 8080).PublishAsContainer();
var DDA = builder.AddProject<Projects.MrX_DynamicDatabaseApi_Api>("mrx-dynamicdatabaseapi-api", c => c.ExcludeLaunchProfile = true);
SQL.WithDataVolume("SQL");
SEQ.WithDataVolume("SEQ");
DDA.WaitFor(DB);
DDA.WaitFor(SEQ);
DDA.WithReference(DB);
DDA.WithReference(SEQ);
//DDA.WithHttpEndpoint(80);
DDA.WithExternalHttpEndpoints();
DDA.WithArgs("--urls", "http://[::]:80");
DDA.AsHttp2Service();
builder.Build().Run();
