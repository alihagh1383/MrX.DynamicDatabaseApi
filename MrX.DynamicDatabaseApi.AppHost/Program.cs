using Json.More;
using Json.Patch;
//Environment.SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", "http://localhost:19218");
//Environment.SetEnvironmentVariable("DOTNET_RESOURCE_SERVICE_ENDPOINT_URL", "http://localhost:20277");
//Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:15205");
var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions() { AllowUnsecuredTransport = true });
var SQLPass = builder.AddParameter("PASS", "sqlpasswordwnttyutymi78674js5rhtty", true);
var SQL = builder.AddMySql("SQL", port: 8000,password: SQLPass).PublishAsContainer();
var DB = SQL.AddDatabase("DB");
var SEQ = builder.AddSeq("SEQ", 8080).PublishAsContainer();
var DDA = builder.AddProject<Projects.MrX_DynamicDatabaseApi_Api>("mrx-dynamicdatabaseapi-api", /*"""E:\MrX\MrX.DynamicDatabaseApi\MrX.DynamicDatabaseApi.Api\MrX.DynamicDatabaseApi.Api.csproj""",*/ c => c.ExcludeLaunchProfile = true);
SQL.WithDataVolume("SQL");
SEQ.WithDataVolume("SEQ");
DDA.WaitFor(DB);
DDA.WaitFor(SEQ);
DDA.WithReference(DB);
DDA.WithReference(SEQ);
DDA.WithExternalHttpEndpoints();
DDA.AsHttp2Service();


var urls = "";
for (int i = 0; i < args.Length; i++)
{
    if (args[i].ToLower() == "--port" && args.Length > i)
    {
        foreach (var arg in args[i + 1].Split(','))
            if (int.TryParse(arg, out int Port) && Port > 0 && Port < 65000)
            {
                urls += $";http://*:{Port}";
                DDA.WithEndpoint(port: Port, targetPort: Port, scheme: "http", name: $"porthttp{Port}", isProxied: false, isExternal: true);
            }
    }
}
if (!string.IsNullOrWhiteSpace(urls))
    DDA.WithArgs("--urls", urls.Trim(';'));



var app = builder.Build();
app.Run();



