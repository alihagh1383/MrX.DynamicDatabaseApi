using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MrX.Web.Logger;
using MrX.Web.Extension;
using MrX.DynamicDatabaseApi;

/*
 Table + Name  => TableRole
 _     + Name  => Deleted
 */
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.AddRedisOutputCache("Cache");
builder.AddSqlServerDbContext<MrX.DynamicDatabaseApi.Database.SQLDBContext>("DB");
builder.Services.AddDbContext<MrX.DynamicDatabaseApi.Database.InMemoryDBContext>(o => o.UseInMemoryDatabase("InMemory"));
builder.Services.AddSingleton<SecurityLogger>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => { policyBuilder.AllowAnyOrigin(); }));

var rule = new AuthorizationPolicyBuilder().AddRequirements(new MrX.DynamicDatabaseApi.Api.Handler.RuleAuthorizationRequirementInput()).Build();
builder.Services.AddAuthorization(o => o.AddPolicy("Rule", rule));
builder.Services.AddSingleton<IAuthorizationHandler, MrX.DynamicDatabaseApi.Api.Handler.RoleAuthorization>();


var app = builder.Build();
app.UseCors();
app.UseHttpsRedirection();
app.UseOutputCache();
app.MapDefaultEndpoints();
app.UseExceptionHandler();
app.UseMiddleware<MrX.Web.Middleware.SetupLogMiddleware>();
app.UseMiddleware<MrX.Web.Middleware.LogRequestCMD>();
app.UseMiddlewareForPath<MrX.Web.Middleware.LogResponseBody>("/Login");
app.UseAuthentication();
app.UseAuthorization();
app.Map("/", () => (Guid.NewGuid()));
var login = app.MapGroup("/{Session:guid}").RequireAuthorization();
login.Map("/", () => (Guid.NewGuid())).RequireAuthorization(rule);
app.Run();

