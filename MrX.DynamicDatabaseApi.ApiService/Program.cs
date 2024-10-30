using MrX.Web.Logger;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<SecurityLogger>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.UseMiddleware<MrX.Web.Middleware.SetupLogMiddleware>();
app.UseMiddleware<MrX.Web.Middleware.LogRequestCMD>();




app.Run();
