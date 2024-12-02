using Confluent.Kafka;
using Projects;

Environment.SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", "http://localhost:19218");
Environment.SetEnvironmentVariable("DOTNET_RESOURCE_SERVICE_ENDPOINT_URL", "http://localhost:20277");
Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:15203");
if (args.Length == 0) args = new string[2] { "--port", "80" };

var builder =
    DistributedApplication.CreateBuilder(new DistributedApplicationOptions { AllowUnsecuredTransport = true });


var SQL = builder.AddMySql("SQL", builder.AddParameter("PASS", "fae3d4c61ac44bf68b0ca984032e17ad", true))
    .PublishAsContainer();
SQL.WithDataVolume("SQL");
SQL.WithLifetime(ContainerLifetime.Persistent);

var DB = SQL.AddDatabase("DB");


var SEQ = builder.AddSeq("SEQ", 8080).PublishAsContainer();
SEQ.WithDataVolume("SEQ");
SEQ.WithLifetime(ContainerLifetime.Persistent);


var LOG = builder.AddKafka("LOG").WithKafkaUI(c => c.WithHostPort(8088)).PublishAsContainer();
LOG.WithDataVolume("LOG");
LOG.WithLifetime(ContainerLifetime.Persistent);
LOG.WithClearCommand();

var DDA = builder.AddProject<MrX_DynamicDatabaseApi_Api>(
    "dda", /*"""E:\MrX\MrX.DynamicDatabaseApi\MrX.DynamicDatabaseApi.Api\MrX.DynamicDatabaseApi.Api.csproj""",*/
    c => c.ExcludeLaunchProfile = true);
DDA.WaitFor(DB);
DDA.WaitFor(SEQ);
DDA.WaitFor(LOG);
DDA.WithReference(DB);
DDA.WithReference(SEQ);
DDA.WithReference(LOG);
DDA.AsHttp2Service();
DDA.ExternalHttpEndpointsWithargs(args);

var app = builder.Build();
app.Run();


internal static class KAFKAResourceBuilderExtensions
{
    public static IResourceBuilder<KafkaServerResource> WithClearCommand(
        this IResourceBuilder<KafkaServerResource> builder)
    {
        return builder.WithCommand("clear", "Clear",
            async context =>
            {
                var connectionString =
                    await builder.Resource.ConnectionStringExpression.GetValueAsync(new CancellationToken()) ??
                    throw new InvalidOperationException(
                        $"Unable to get the '{context.ResourceName}' connection string.");
                using (var admin = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = connectionString })
                           .Build())
                {
                    var meta = admin.GetMetadata(TimeSpan.FromSeconds(30));
                    if (!(meta.Topics == null || meta.Topics.Count == 0))
                        await admin.DescribeTopicsAsync(TopicCollection.OfTopicNames(meta.Topics.Select(p => p.Topic)));
                }

                return CommandResults.Success();
            },
            iconName: "Delete", iconVariant: IconVariant.Regular, isHighlighted: true);
    }
}

internal static class ProjectResourceBuilderExtensions
{
    public static IResourceBuilder<ProjectResource> ExternalHttpEndpointsWithargs(
        this IResourceBuilder<ProjectResource> builder, params string[] args)
    {
        var urls = "";
        var ports = new List<int>();
        for (var i = 0; i < args.Length; i++)
            if (args[i].ToLower() == "--port" && args.Length > i)
                foreach (var arg in args[i + 1].Split(','))
                    if (int.TryParse(arg, out var Port) && Port > 0 && Port < 65000)
                        ports.Add(Port);
        if (ports.Count == 0) throw new Exception("no port in --port (ex : --port 80,8080)");
        urls += $";http://*:{ports.Last()}";
        builder.WithEndpoint(ports.Last(), ports.Last(), "http", isProxied: false, isExternal: true);
        ports.Remove(ports.Last());
        foreach (var Port in ports)
        {
            urls += $";http://*:{Port}";
            builder.WithEndpoint(Port, Port, "http", $"porthttp{Port}", isProxied: false, isExternal: true);
        }

        if (!string.IsNullOrWhiteSpace(urls))
            builder.WithArgs("--urls", urls.Trim(';'));
        builder.WithExternalHttpEndpoints();
        return builder;
    }
}