using IdentityModel;
using IdentityServer;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using System.Security.Claims;

namespace Server
{
	public class SeedData
	{
		public static void EnsureSeedData(string connectionString, IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<AspNetIdentityDbContext>(
                options => options.UseSqlServer(connectionString)
            );

            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddOperationalDbContext(
                options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlServer(
                            connectionString,
                            sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                        );
                }
            );
            services.AddConfigurationDbContext(
                options =>
                {
                    options.ConfigureDbContext = db =>
                        db.UseSqlServer(
                            connectionString,
                            sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName)
                        );
                }
            );

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

            var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            context.Database.Migrate();

            EnsureSeedData(context, configuration);

            var ctx = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
            ctx.Database.Migrate();
            EnsureUsers(scope);
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var userRole = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminRole = userRole.FindByNameAsync("Admin").Result;

            if (adminRole == null)
            {
                adminRole = new IdentityRole { Name = "Admin" };
                var result = userRole.CreateAsync(adminRole).Result;

                if(!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            var regular = userRole.FindByNameAsync("Regular").Result;

            if(regular == null)
            {
                regular = new IdentityRole { Name = "Regular" };
                var result = userRole.CreateAsync(regular).Result;

                if(!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            var angella = userMgr.FindByNameAsync("angella").Result;
            if (angella == null)
            {
                angella = new ApplicationUser
                {
                    UserName = "angella",
                    LastName = "Angella",
                    FirstName = "Angella",
                    Email = "angella.freeman@email.com",
                    EmailConfirmed = true,
                    Address = "1709 Cat Tail Ct",
                    City = "Lawrenceville",
                    State="GA",
                    ZipCode = "30043"
                };
                var result = userMgr.CreateAsync(angella, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result =
                    userMgr.AddClaimsAsync(
                        angella,
                        new Claim[]
                        {
                            new Claim(JwtClaimTypes.Name, "Angella Freeman"),
                            new Claim(JwtClaimTypes.GivenName, "Angella"),
                            new Claim(JwtClaimTypes.FamilyName, "Freeman"),
                            new Claim(JwtClaimTypes.WebSite, "http://angellafreeman.com"),
                            new Claim("location", "somewhere")
                        }
                    ).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddToRoleAsync(angella, "Regular").Result;

                if(!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            var admin = userMgr.FindByNameAsync("admin").Result;

            if(admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    LastName = "admin",
                    FirstName = "admin",
                    Email = "admin@email.com",
                    EmailConfirmed = true,
                    Address = "1709 Cat Tail Ct",
                    City = "Lawrenceville",
                    State = "GA",
                    ZipCode = "30043"
                };

                var result = userMgr.CreateAsync(admin, "Abc@123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddToRoleAsync(admin, "Admin").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

            }
        }

        private static void EnsureSeedData(ConfigurationDbContext context, IConfiguration configuration)
        {
            if (!context.Clients.Any())
            {
                Dictionary<string, string> clientUrl = new Dictionary<string, string>();
                clientUrl.Add("webhookmvc", configuration.GetValue<string>("WebHookClientUrl"));
                clientUrl.Add("mvcclient", configuration.GetValue<string>("MvcClientUrl"));

                foreach (var client in IdentityConfig.Clients(clientUrl).ToList())
                {
                    context.Clients.AddRange(client.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in IdentityConfig.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in IdentityConfig.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

        }
        //private static void EnsureSeedData(ConfigurationDbContext context)
        //{
        //    if (!context.Clients.Any())
        //    {
        //        foreach (var client in Config.Clients.ToList())
        //        {
        //            context.Clients.Add(client.ToEntity());
        //        }

        //        context.SaveChanges();
        //    }

        //    if (!context.IdentityResources.Any())
        //    {
        //        foreach (var resource in Config.IdentityResources())
        //        {
        //            context.IdentityResources.Add(resource.ToEntity());
        //        }

        //        context.SaveChanges();
        //    }

        //    if (!context.ApiScopes.Any())
        //    {
        //        foreach (var resource in Config.ApiScopes.ToList())
        //        {
        //            context.ApiScopes.Add(resource.ToEntity());
        //        }

        //        context.SaveChanges();
        //    }

        //    if (!context.ApiResources.Any())
        //    {
        //        foreach (var resource in Config.ApiResources.ToList())
        //        {
        //            context.ApiResources.Add(resource.ToEntity());
        //        }

        //        context.SaveChanges();
        //    }
        //}
	}
}
