using BookOnline.Aggregator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = GetConfiguration();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
IdentityModelEventSource.ShowPII = true;  // Caution! Do NOT use in production: https://aka.ms/IdentityModel/PII

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = configuration["IdentityUrl"];// https://localhost:5678";
    options.Audience = "aggregator";
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

// adds an authorization policy to make sure the token is for scope 'api1'
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("aggregator", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "aggregator");
    });
});

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

builder.Services.AddGrpcClient<GrpcBasket.Basket.BasketClient>((services, options) =>
{
    var apiUrl = configuration["GrpcBasketUrl"];//"https://localhost:7251";
    
    options.Address = new Uri(apiUrl);
});

builder.Services.AddGrpcClient<GrpcBook.Book.BookClient>((services, options) =>
{
    var apiUrl = configuration["GrpcBookUrl"];//"https://localhost:7275";
    options.Address = new Uri(apiUrl);
});

builder.Services.AddGrpcClient<GrpcBorrow.Borrowing.BorrowingClient>((services, options) =>
{
    var apiUrl = configuration["GrpcBorrowUrl"]; //"https://localhost:7157";
    options.Address = new Uri(apiUrl);
});

builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddSingleton<IBasketService, BasketService>();
builder.Services.AddSingleton<IBorrowService, BorrowService>();

var app = builder.Build();

var pathBase = configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}