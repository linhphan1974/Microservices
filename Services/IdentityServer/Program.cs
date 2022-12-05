using BookOnline.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;

//var seed = args.Contains("/seed");
//if (seed)
//{
//    args = args.Except(new[] { "/seed" }).ToArray();
//}

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly.GetName().Name;
var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");

//if (seed)
//{
//}
builder.Services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
builder.Services.AddTransient<IRedirectService, RedirectService>();

builder.Services.AddDbContext<AspNetIdentityDbContext>(options =>
    options.UseSqlServer(defaultConnString,
        b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AspNetIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<ApplicationUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    })
    .AddDeveloperSigningCredential()
    .Services.AddTransient<IProfileService, ProfileService>();

//builder.Services.AddTransient<IProfileService, ProfileService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//app.Use(async (ctx, next) =>
//{
//    ctx.Request.Scheme = "http";
//    ctx.Request.Host = new HostString("host.docker.internal:8011");

//    await next();
//});

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseIdentityServer();

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapDefaultControllerRoute();
});
SeedData.EnsureSeedData(defaultConnString,builder.Configuration);

app.Run();
