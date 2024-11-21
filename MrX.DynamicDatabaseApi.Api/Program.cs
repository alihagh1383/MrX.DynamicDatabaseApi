using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MrX.Web.Logger;
using MrX.Web.Extension;
using MrX.DynamicDatabaseApi;
using Microsoft.AspNetCore.Identity;
using MrX.DynamicDatabaseApi.Api.Handler;
using MrX.DynamicDatabaseApi.Database;
using MrX.DynamicDatabaseApi.Api.SetupFunction;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MrX.DynamicDatabaseApi.CallBack;
using MrX.DynamicDatabaseApi.Api.Endpoint;
using MrX.DynamicDatabaseApi.Api;
using Microsoft.AspNetCore.Routing;



/*
 Table + Name  => TableRole
 _     + Name  => Deleted
 */
var builder = WebApplication.CreateBuilder(args);
builder.AddSeqEndpoint("SEQ");
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.AddMySqlDbContext<MrX.DynamicDatabaseApi.Database.SQLDBContext>("DB");
//builder.Services.AddDbContext<MrX.DynamicDatabaseApi.Database.SQLDBContext>(o => { o.UseMySql(ServerVersion.AutoDetect("server=127.0.0.1; Port=30000;uid=root;pwd=ali1383ali@;database=DB"),m=>m.EnableRetryOnFailure()); });
builder.Services.AddDbContext<MrX.DynamicDatabaseApi.Database.InMemoryDBContext>(o => o.UseInMemoryDatabase("InMemory"));
builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => { policyBuilder.AllowAnyOrigin(); }));

var rule = new AuthorizationPolicyBuilder().AddRequirements(new MrX.DynamicDatabaseApi.Api.Handler.RuleAuthorizationRequirementInput()).Build();
builder.Services.AddAuthorization(o => o.AddPolicy("Rule", rule));
builder.Services.AddSingleton<IAuthorizationHandler, MrX.DynamicDatabaseApi.Api.Handler.RoleAuthorization>();

builder.Services.AddAuthentication().AddScheme<CustomBasicAuthenticationSchemeOptions, LoginAuthentication>("SessionTokens", null);

builder.Services.AddScoped<MrX.DynamicDatabaseApi.Worker.DBWFields>();
builder.Services.AddScoped<MrX.DynamicDatabaseApi.Worker.DBWLogins>();
builder.Services.AddScoped<MrX.DynamicDatabaseApi.Worker.DBWRoles>();
builder.Services.AddScoped<MrX.DynamicDatabaseApi.Worker.DBWTabels>();
builder.Services.AddScoped<MrX.DynamicDatabaseApi.Worker.DBWUser>();

builder.Services.ConfigureHttpJsonOptions(c => c.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddSingleton<SecurityLogger>((SP) =>
{
    var F = SP.GetService<ILoggerFactory>()!;
    return new SecurityLogger(F, false, true);
});

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
//app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseMiddleware<MrX.Web.Middleware.SetupLogMiddleware>();
app.UseMiddleware<MrX.Web.Middleware.LogRequestCMD>();
app.UseMiddlewareForPaths<MrX.Web.Middleware.LogResponseBody>("/Login");

app.Map("/", (HttpContext hc)=> (new Return(Return.Loc.Check, 200, Guid.CreateVersion7().ToString(), hc.Items.Where(c => !(c.Key.ToString()?.StartsWith("__") ?? true)).ToDictionary())));




app.Map("/login/{UserNameOrEmail}/{Password}", MrX.DynamicDatabaseApi.Api.Endpoint.Authentication.Login);
var login = app.MapGroup("/{Session:guid}").RequireAuthorization();
login.Map("/", (HttpContext hc) => new Return(Return.Loc.Check, 200, (hc.Items["LoginId"] ?? "").ToString() ?? "", hc.Items["user"])).RequireAuthorization(rule);


{
    Static.RouteLogin.Add(new("UserMy", new("Users/My", UserEndpoint.UserMy)));
    Static.RouteRule.Add(new("UserAdd", new("Users/Add", UserEndpoint.UserAdd)));
    Static.RouteLogin.Add(new("UserGet", new("Users/Get", UserEndpoint.UserGet)));
    Static.RouteRule.Add(new("UserList", new("Users/List", UserEndpoint.UserList)));
    Static.RouteRule.Add(new("UserRemove", new("Users/Remove", UserEndpoint.UserRemove)));
    Static.RouteRule.Add(new("UserUpdate", new("Users/Update", UserEndpoint.UserUpdate)));
    Static.RouteRule.Add(new("UserAddData", new("Users/AddData", UserEndpoint.UserAddData)));
    Static.RouteRule.Add(new("UserAddRole", new("Users/AddRole", UserEndpoint.UserAddRole)));
    Static.RouteRule.Add(new("UserRemoveRole", new("Users/RemoveRole", UserEndpoint.UserRemoveRole)));
    Static.RouteRule.Add(new("UserRemoveData", new("Users/RemoveData", UserEndpoint.UserRemoveData)));
    Static.RouteRule.Add(new("UserPublicList", new("Users/ListPublic", UserEndpoint.UserPublicList)));
    Static.RouteRule.Add(new("UserCheangeParent", new("Users/ParentChenge", UserEndpoint.UserCheangeParent)));
    Static.RouteLogin.Add(new("UserUpdatePassword", new("Users/UpdatePassword", UserEndpoint.UserUpdatePassword)));
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
    Static.RouteRule.Add(new("TablesAdd", new("/Tables/Add", TablesEndpoint.TablesAdd)));
    Static.RouteLogin.Add(new("TablesGet", new("/Tables/Get", TablesEndpoint.TablesGet)));
    Static.RouteLogin.Add(new("TablesList", new("/Tables/List", TablesEndpoint.TablesList)));
    Static.RouteLogin.Add(new("TablesRemove", new("/Tables/Remove", TablesEndpoint.TablesRemove)));
    Static.RouteLogin.Add(new("TablesAddField", new("/Tables/AddField", TablesEndpoint.TablesAddField)));
    Static.RouteLogin.Add(new("TablesGetField", new("/Tables/GetField", TablesEndpoint.TablesGetField)));
    Static.RouteLogin.Add(new("TablesRemoveField", new("/Tables/RemoveField", TablesEndpoint.TablesRemoveField)));
    Static.RouteLogin.Add(new("TablesUpdateField", new("/Tables/UpdateField", TablesEndpoint.TablesUpdateField)));
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
    Static.RouteLogin.Add(new("TableAdd", new("/Table/Add", TableEndpoint.TableAdd)));
    Static.RouteLogin.Add(new("TableGet", new("/Table/Get", TableEndpoint.TableGet)));
    Static.RouteLogin.Add(new("TableList", new("/Table/List", TableEndpoint.TableList)));
    Static.RouteLogin.Add(new("TableRemove", new("/Table/Remove", TableEndpoint.TableRemove)));
    Static.RouteLogin.Add(new("TableAddField", new("/Table/AddField", TableEndpoint.TableAddField)));
    Static.RouteLogin.Add(new("TableGetField", new("/Table/GetField", TableEndpoint.TableGetField)));
    Static.RouteLogin.Add(new("TableRemoveField", new("/Table/RemoveField", TableEndpoint.TableRemoveField)));
    Static.RouteLogin.Add(new("TableUpdateField", new("/Table/UpdateField", TableEndpoint.TableUpdateField)));
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
    Static.RouteRule.Add(new("RoleAdd", new("Roles/Add", RoleEndpoint.RoleAdd)));
    Static.RouteRule.Add(new("RoleAddField", new("Roles/AddField", RoleEndpoint.RoleAddField)));
    Static.RouteRule.Add(new("RoleAddTable", new("Roles/AddTable", RoleEndpoint.RoleAddTable)));
    Static.RouteRule.Add(new("RoleAddUser", new("Roles/AddUser", RoleEndpoint.RoleAddUser)));
    Static.RouteRule.Add(new("RoleAddPath", new("Roles/AddPath", RoleEndpoint.RoleAddPath)));
    Static.RouteRule.Add(new("RoleRemove", new("Roles/Remove", RoleEndpoint.RoleRemove)));
    Static.RouteRule.Add(new("RoleRemoveField", new("Roles/RemoveField", RoleEndpoint.RoleRemoveField)));
    Static.RouteRule.Add(new("RoleRemoveTable", new("Roles/RemoveTable", RoleEndpoint.RoleRemoveTable)));
    Static.RouteRule.Add(new("RoleRemoveUser", new("Roles/RemoveUser", RoleEndpoint.RoleRemoveUser)));
    Static.RouteRule.Add(new("RoleRemovePath", new("Roles/RemovePath", RoleEndpoint.RoleRemovePath)));
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