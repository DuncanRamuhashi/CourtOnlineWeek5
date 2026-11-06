using CourtOnlineWeek5.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CourtCaseDatabaseContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //inject the DB context into the DI container



var app = builder.Build();

// fo bootstrap 5 we need to add the following line to the _Layout.cshtml file
app.UseStaticFiles(); // this is for the static files like css, js, images etc


app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
