using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MrX.DynamicDatabaseApi.Api.Endpoint;
using MrX.DynamicDatabaseApi.Api.Handler;
using MrX.DynamicDatabaseApi.Api.SetupFunction;
using MrX.DynamicDatabaseApi.CallBack;
using MrX.DynamicDatabaseApi.Database;
using MrX.DynamicDatabaseApi.Worker;
using MrX.Web.Extension;
using MrX.Web.Logger;
using MrX.Web.Middleware;
using Static = MrX.DynamicDatabaseApi.Api.Static;


/*
 Table + Name  => TableRole
 _     + Name  => Deleted
 */
var builder = WebApplication.CreateBuilder(args);
builder.AddSeqEndpoint("SEQ");
builder.AddKafkaProducer<string, string>("LOG");
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.AddMySqlDbContext<SQLDBContext>("DB");
//builder.Services.AddDbContext<MrX.DynamicDatabaseApi.Database.SQLDBContext>(o => { o.UseMySql(ServerVersion.AutoDetect("server=127.0.0.1; Port=30000;uid=root;pwd=ali1383ali@;database=DB"),m=>m.EnableRetryOnFailure()); });
builder.Services.AddDbContext<InMemoryDBContext>(o => o.UseInMemoryDatabase("InMemory"));
builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => { policyBuilder.AllowAnyOrigin(); }));

var rule = new AuthorizationPolicyBuilder().AddRequirements(new RuleAuthorizationRequirementInput()).Build();
builder.Services.AddAuthorization(o => o.AddPolicy("Rule", rule));
builder.Services.AddSingleton<IAuthorizationHandler, RoleAuthorization>();

builder.Services.AddAuthentication()
    .AddScheme<CustomBasicAuthenticationSchemeOptions, LoginAuthentication>("SessionTokens", null);

builder.Services.AddScoped<DBWFields>();
builder.Services.AddScoped<DBWLogins>();
builder.Services.AddScoped<DBWRoles>();
builder.Services.AddScoped<DBWTabels>();
builder.Services.AddScoped<DBWUser>();

builder.Services.ConfigureHttpJsonOptions(c => c.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<SecurityLogger>(SP =>
{
    var C = SP.GetService<IProducer<string, string>>()!;
    var F = SP.GetService<ILoggerFactory>()!;
    return new SecurityLogger(F, false, false,
        (i, s, a) =>
        {
            C.Produce("SecurityLogger", new Message<string, string> { Key = i, Value = s.ToString() ?? "" });
        });
});

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
//app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseMiddleware<SetupLogMiddleware>();
app.UseMiddleware<LogRequestCMD>();
app.UseMiddlewareForPaths<LogResponseBody>("/Login");


var root = app.MapGroup("/").AllowAnonymous();
root.Map("/",
    (HttpContext hc) => new Return(Return.Loc.Check, 200, Guid.CreateVersion7().ToString(),
        hc.Items.Where(c => !(c.Key.ToString()?.StartsWith("__") ?? true)).ToDictionary()));
root.Map("/login/{UserNameOrEmail}/{Password}", Authentication.Login);
var login = root.MapGroup("/{Session:guid}").RequireAuthorization();
login.Map("/",
        (HttpContext hc) =>
            new Return(Return.Loc.Check, 200, (hc.Items["LoginId"] ?? "").ToString() ?? "", hc.Items["user"]))
    .RequireAuthorization(rule);


{
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserMy",
        new KeyValuePair<string, Delegate>("Users/My", UserEndpoint.UserMy)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserAdd",
        new KeyValuePair<string, Delegate>("Users/Add", UserEndpoint.UserAdd)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserGet",
        new KeyValuePair<string, Delegate>("Users/Get", UserEndpoint.UserGet)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserList",
        new KeyValuePair<string, Delegate>("Users/List", UserEndpoint.UserList)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserRemove",
        new KeyValuePair<string, Delegate>("Users/Remove", UserEndpoint.UserRemove)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserUpdate",
        new KeyValuePair<string, Delegate>("Users/Update", UserEndpoint.UserUpdate)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserAddData",
        new KeyValuePair<string, Delegate>("Users/AddData", UserEndpoint.UserAddData)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserAddRole",
        new KeyValuePair<string, Delegate>("Users/AddRole", UserEndpoint.UserAddRole)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserRemoveRole",
        new KeyValuePair<string, Delegate>("Users/RemoveRole", UserEndpoint.UserRemoveRole)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserRemoveData",
        new KeyValuePair<string, Delegate>("Users/RemoveData", UserEndpoint.UserRemoveData)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserPublicList",
        new KeyValuePair<string, Delegate>("Users/ListPublic", UserEndpoint.UserPublicList)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserCheangeParent",
        new KeyValuePair<string, Delegate>("Users/ParentChenge", UserEndpoint.UserCheangeParent)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("UserUpdatePassword",
        new KeyValuePair<string, Delegate>("Users/UpdatePassword", UserEndpoint.UserUpdatePassword)));
    login.Map("Users/My", UserEndpoint.UserMy).RequireAuthorization();
    login.Map("Users/Add", UserEndpoint.UserAdd).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/Get", UserEndpoint.UserGet).RequireAuthorization();
    login.Map("Users/List", UserEndpoint.UserList).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/Remove", UserEndpoint.UserRemove).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/Update", UserEndpoint.UserUpdate).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/AddData", UserEndpoint.UserAddData).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/AddRole", UserEndpoint.UserAddRole).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/RemoveRole", UserEndpoint.UserRemoveRole).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/RemoveData", UserEndpoint.UserRemoveData).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/ListPublic", UserEndpoint.UserPublicList).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/ParentChenge", UserEndpoint.UserCheangeParent).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Users/UpdatePassword", UserEndpoint.UserUpdatePassword).RequireAuthorization();
}
{
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TablesAdd",
        new KeyValuePair<string, Delegate>("/Tables/Add", TablesEndpoint.TablesAdd)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TablesGet",
        new KeyValuePair<string, Delegate>("/Tables/Get", TablesEndpoint.TablesGet)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TablesList",
        new KeyValuePair<string, Delegate>("/Tables/List", TablesEndpoint.TablesList)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TablesRemove",
        new KeyValuePair<string, Delegate>("/Tables/Remove", TablesEndpoint.TablesRemove)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TablesAddField",
        new KeyValuePair<string, Delegate>("/Tables/AddField", TablesEndpoint.TablesAddField)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TablesGetField",
        new KeyValuePair<string, Delegate>("/Tables/GetField", TablesEndpoint.TablesGetField)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TablesRemoveField",
        new KeyValuePair<string, Delegate>("/Tables/RemoveField", TablesEndpoint.TablesRemoveField)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TablesUpdateField",
        new KeyValuePair<string, Delegate>("/Tables/UpdateField", TablesEndpoint.TablesUpdateField)));
    login.Map("/Tables/Add", TablesEndpoint.TablesAdd).RequireAuthorization().RequireAuthorization(rule);
    login.Map("/Tables/Get", TablesEndpoint.TablesGet).RequireAuthorization();
    login.Map("/Tables/List", TablesEndpoint.TablesList).RequireAuthorization();
    login.Map("/Tables/Remove", TablesEndpoint.TablesRemove).RequireAuthorization();
    login.Map("/Tables/AddField", TablesEndpoint.TablesAddField).RequireAuthorization();
    login.Map("/Tables/GetField", TablesEndpoint.TablesGetField).RequireAuthorization();
    login.Map("/Tables/RemoveField", TablesEndpoint.TablesRemoveField).RequireAuthorization();
    login.Map("/Tables/UpdateField", TablesEndpoint.TablesUpdateField).RequireAuthorization();
}
{
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TableAdd",
        new KeyValuePair<string, Delegate>("/Table/Add", TableEndpoint.TableAdd)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TableGet",
        new KeyValuePair<string, Delegate>("/Table/Get", TableEndpoint.TableGet)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TableList",
        new KeyValuePair<string, Delegate>("/Table/List", TableEndpoint.TableList)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TableRemove",
        new KeyValuePair<string, Delegate>("/Table/Remove", TableEndpoint.TableRemove)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TableAddField",
        new KeyValuePair<string, Delegate>("/Table/AddField", TableEndpoint.TableAddField)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TableGetField",
        new KeyValuePair<string, Delegate>("/Table/GetField", TableEndpoint.TableGetField)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TableRemoveField",
        new KeyValuePair<string, Delegate>("/Table/RemoveField", TableEndpoint.TableRemoveField)));
    Static.RouteLogin.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("TableUpdateField",
        new KeyValuePair<string, Delegate>("/Table/UpdateField", TableEndpoint.TableUpdateField)));
    login.Map("/Table/Add", TableEndpoint.TableAdd).RequireAuthorization();
    login.Map("/Table/Get", TableEndpoint.TableGet).RequireAuthorization();
    login.Map("/Table/List", TableEndpoint.TableList).RequireAuthorization();
    login.Map("/Table/Remove", TableEndpoint.TableRemove).RequireAuthorization();
    login.Map("/Table/AddField", TableEndpoint.TableAddField).RequireAuthorization();
    login.Map("/Table/GetField", TableEndpoint.TableGetField).RequireAuthorization();
    login.Map("/Table/RemoveField", TableEndpoint.TableRemoveField).RequireAuthorization();
    login.Map("/Table/UpdateField", TableEndpoint.TableUpdateField).RequireAuthorization();
}
{
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleAdd",
        new KeyValuePair<string, Delegate>("Roles/Add", RoleEndpoint.RoleAdd)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleAddField",
        new KeyValuePair<string, Delegate>("Roles/AddField", RoleEndpoint.RoleAddField)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleAddTable",
        new KeyValuePair<string, Delegate>("Roles/AddTable", RoleEndpoint.RoleAddTable)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleAddUser",
        new KeyValuePair<string, Delegate>("Roles/AddUser", RoleEndpoint.RoleAddUser)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleAddPath",
        new KeyValuePair<string, Delegate>("Roles/AddPath", RoleEndpoint.RoleAddPath)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleRemove",
        new KeyValuePair<string, Delegate>("Roles/Remove", RoleEndpoint.RoleRemove)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleRemoveField",
        new KeyValuePair<string, Delegate>("Roles/RemoveField", RoleEndpoint.RoleRemoveField)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleRemoveTable",
        new KeyValuePair<string, Delegate>("Roles/RemoveTable", RoleEndpoint.RoleRemoveTable)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleRemoveUser",
        new KeyValuePair<string, Delegate>("Roles/RemoveUser", RoleEndpoint.RoleRemoveUser)));
    Static.RouteRule.Add(new KeyValuePair<string, KeyValuePair<string, Delegate>>("RoleRemovePath",
        new KeyValuePair<string, Delegate>("Roles/RemovePath", RoleEndpoint.RoleRemovePath)));
    login.Map("Roles/Get", RoleEndpoint.RoleGet).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/List", RoleEndpoint.RoleList).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/Add", RoleEndpoint.RoleAdd).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/AddField", RoleEndpoint.RoleAddField).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/AddTable", RoleEndpoint.RoleAddTable).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/AddUser", RoleEndpoint.RoleAddUser).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/AddPath", RoleEndpoint.RoleAddPath).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/Remove", RoleEndpoint.RoleRemove).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/RemoveField", RoleEndpoint.RoleRemoveField).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/RemoveTable", RoleEndpoint.RoleAddTable).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/RemoveUser", RoleEndpoint.RoleRemoveUser).RequireAuthorization().RequireAuthorization(rule);
    login.Map("Roles/RemovePath", RoleEndpoint.RoleRemovePath).RequireAuthorization().RequireAuthorization(rule);
}


app.OnRunDB();
app.Run();


while (true) ;