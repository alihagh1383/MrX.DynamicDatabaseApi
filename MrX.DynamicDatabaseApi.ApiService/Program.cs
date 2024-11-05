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

/*
 Table + Name  => TableRole
 _     + Name  => Deleted
 */
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.AddSqlServerDbContext<MrX.DynamicDatabaseApi.Database.SQLDBContext>("DB");
//builder.Services.AddDbContext<MrX.DynamicDatabaseApi.Database.SQLDBContext>(o => { o.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CM2;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False").ConfigureWarnings(builder => builder.Ignore(InMemoryEventId.TransactionIgnoredWarning)); });
builder.Services.AddDbContext<MrX.DynamicDatabaseApi.Database.InMemoryDBContext>(o => o.UseInMemoryDatabase("InMemory"));
builder.Services.AddSingleton<SecurityLogger>();
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

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
//app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.UseExceptionHandler();
app.UseMiddleware<MrX.Web.Middleware.SetupLogMiddleware>();
app.UseMiddleware<MrX.Web.Middleware.LogRequestCMD>();
app.UseMiddlewareForPaths<MrX.Web.Middleware.LogResponseBody>("/Login");


app.Map("/", (HttpContext hc) => new Return(Return.Loc.Check, 200, Guid.NewGuid().ToString(), hc.Items.Where(c => !((c.Key.ToString()?.StartsWith("__")??true))).ToDictionary()));
app.Map("/login/{UserNameOrEmail}/{Password}", MrX.DynamicDatabaseApi.Api.Endpoint.Authentication.Login);
var login = app.MapGroup("/{Session}").RequireAuthorization();
login.Map("/", (HttpContext hc) => new Return(Return.Loc.Check, 200, (hc.Items["LoginId"] ?? "").ToString() ?? "", hc.Items["user"])).RequireAuthorization(rule);



app.OnRunDB();
app.Run();