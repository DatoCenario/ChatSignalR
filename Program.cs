using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<HttpContextAccessor>();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.ConfigureGraphQl();

builder.Services.AddDbContext<ApplicationContext>(o => o
    .UseNpgsql(builder.Configuration.GetValue<string>("PgDbConnectionString")));

builder.Services.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(o =>
{
    o.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
    o.Lockout = new LockoutOptions
    {
        MaxFailedAccessAttempts = 100000
    };
    o.SignIn.RequireConfirmedEmail = false;
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<ApplicationContext>();
    context.Database.Migrate();
}

app.Run();