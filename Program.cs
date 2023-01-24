using System.Reflection;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2;
using WebApplication2.Data.EF;
using WebApplication2.Data.EF.Domain;
using WebApplication2.Extensions;
using WebApplication2.Hubs;
using WebApplication2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<HttpContextAccessor>();

builder.Services.AddScoped<MessagesService>();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.ConfigureGraphQl();

builder.Services.AddDbContext<ApplicationContext>(o => o
    .UseNpgsql(Environment.GetEnvironmentVariable("EF_CORE_CONN") ?? builder.Configuration.GetValue<string>("PgDbConnectionString")), ServiceLifetime.Scoped);

builder.Services.AddIdentity<User, IdentityRole<long>>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = null;
});

builder.Services.Configure<IdentityOptions>(o =>
{
    o.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Lockout = new LockoutOptions
    {
        MaxFailedAccessAttempts = 100000
    };
    o.SignIn.RequireConfirmedEmail = false;
});

builder.Services.AddAuthorization();

var config = new TypeAdapterConfig();
MapsterConfigure.Configure(config);
builder.Services.AddSingleton(config);


var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.MapGraphQL();
app.MapHub<MessengerHub>("/hub/messengerHub");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<ApplicationContext>();

    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

app.Run();