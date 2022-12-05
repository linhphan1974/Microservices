using BookOnline.MvcClient;
using BookOnline.MvcClient.HttpClientHelper;
using BookOnline.MvcClient.Models;
using BookOnline.MvcClient.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = GetConfiguration();
// Add services to the container.
IdentityModelEventSource.ShowPII = true;  // Caution! Do NOT use in production: https://aka.ms/IdentityModel/PII

builder.Services.AddControllersWithViews(option => option.AllowEmptyInputInBodyModelBinding = true);

builder.Services.AddTransient<ClientHeaderDelegatingHandler>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddOptions();
builder.Services.Configure<AppSettings>(configuration);
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(Timeout.InfiniteTimeSpan);

builder.Services.AddHttpClient("MVCClient")
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddHttpMessageHandler<ClientHeaderDelegatingHandler>();
        //.ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        //{
        //    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        //}); 

//builder.Services.AddHttpClient<IBookService, BookService>(client =>
//    {
//        client.DefaultRequestHeaders.Clear();
//        client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
//    })
//    .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
//    .AddHttpMessageHandler<ClientHeaderDelegatingHandler>();

//builder.Services.AddHttpClient<IBasketService, BasketService>(client =>
//    {
//        client.DefaultRequestHeaders.Clear();
//        client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
//    })
//    .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
//    .AddHttpMessageHandler<ClientHeaderDelegatingHandler>();

builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<IBasketService, BasketService>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddTransient<IBorrowService, BorrowService>();
builder.Services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

var identityUrl = configuration.GetValue<string>("IdentityUrl");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = identityUrl.ToString();
        options.ClientId = "mvc";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.RequireHttpsMetadata = false;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("borrow");
        options.Scope.Add("basket");
        options.Scope.Add("aggregator");
        options.Scope.Add("book");
        options.Scope.Add("signalr");
        options.SaveTokens = true;
        //options.BackchannelHttpHandler = new HttpClientHandler()
        //{
        //    // CAUTION USING THIS !!!
        //    ServerCertificateCustomValidationCallback =
        //    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        //};
    });

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>().AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}
