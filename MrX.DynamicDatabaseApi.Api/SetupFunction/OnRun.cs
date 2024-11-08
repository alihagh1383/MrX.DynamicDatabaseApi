using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MrX.Security;

namespace MrX.DynamicDatabaseApi.Api.SetupFunction
{
    public static class OnRun
    {
        public static WebApplication OnRunDB(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<Database.SQLDBContext>();
                   // db.Database.Migrate();
                    var UM = services.GetRequiredService<Worker.DBWUser>();
                    var TM = services.GetRequiredService< Worker.DBWTabels> ();
                    var FM = services.GetRequiredService< Worker.DBWFields> ();
                    var RM = services.GetRequiredService< Worker.DBWRoles> ();
                    var Paths = Static.RouteRule.Select(p => p.Key).ToList();
                    var Tabels = TM.GetAll();
                    var Filds = FM.GetAll();
                    var idu = new Guid("42f5d92e-7bec-4644-bb6b-c6d9d3416935");
                    var idr = new Guid("fd2137ba-8a6b-45e2-ad74-1ea5a3995025");
                    if (!UM.Exist(p => p.Id == idu)) { UM.Add(new() { Id = idu, UserName = "Admin", Password = PasswordHash.Hash("Admin"), }); }
                    var U = UM.Find(idu);
                    if (!RM.Exist(p => p.Id == idr)) { RM.Add(new() { Id = idr, Name = "Admin", Creator = U }); };
                    var R = RM.Find(idr);
                    R!.Users.Add(U!);
                    R.FieldsAddRole.AddRange(Filds);
                    R.FieldsDeleteRole.AddRange(Filds);
                    R.FieldsUpdateRole.AddRange(Filds);
                    R.FieldsReadRole.AddRange(Filds);
                    R.TabelsAddRole.AddRange(Tabels);
                    R.TabelsDeleteRole.AddRange(Tabels);
                    R.TabelsUpdateRole.AddRange(Tabels);
                    R.TabelsReadRole.AddRange(Tabels);
                    UM.Update(U!);
                    RM.Update(R!);
                    var Memory = services.GetRequiredService< Worker.DBWLogins> ();
                    Memory.sync();
                   Worker.Static.ConStr = db.Database.GetConnectionString()!;
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            return app;
        }

    }
}
