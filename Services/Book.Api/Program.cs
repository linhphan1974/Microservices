using Book.Api.Infrastucture.Data;
using BookOnline.Book.Api;
using BookOnline.Book.Api.GrpcService;
using BookOnline.Book.Api.Infrastucture;
using BookOnline.Book.Api.Infrastucture.Data;
using BookOnline.Book.Api.Infrastucture.EventHandlers;
using BookOnline.Book.Api.Infrastucture.Events;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using RabbitMQEventBus;
using Serilog;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = "Picture"
});

var configuration = GetConfiguration();

builder.WebHost.ConfigureKestrel(options =>
{
    var ports = GetDefinedPorts(configuration);
    options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
    options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});


// Add services to the container.
//Create seri log
Log.Logger = CreateSerilogLogger(configuration);

builder.Services.AddControllers(option => option.AllowEmptyInputInBodyModelBinding = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions();
builder.Services.Configure<BookSettings>(configuration);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});


builder.Services.AddEntityFrameworkSqlServer()
    .AddDbContext<BookDBContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("BookDB"),
                                        sqlServerOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                        });
    }, ServiceLifetime.Transient, ServiceLifetime.Singleton
    );

builder.Services.AddTransient<IBookRepository, BookRepository>();
//RabbitMQ

builder.Services.AddSingleton<IRabbitMQConncection, RabbitMQConnection>(sp =>
{
    ConnectionFactory factory = new ConnectionFactory();
    factory.HostName = configuration["EventHost"];//"rabbitmq";//
    factory.Port = 5672;
    factory.UserName = "guest";
    factory.Password = "guest";
    factory.DispatchConsumersAsync = true;
    return new RabbitMQConnection(factory, 3);
});


builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>(sp =>
{
    var queueName = configuration["QueueName"];
    var rabbitMQConnection = sp.GetRequiredService<IRabbitMQConncection>();

    var eventSubcription = sp.GetRequiredService<IEventSubcriptionManager>();

    return new RabbitMQEventBus.RabbitMQEventBus(rabbitMQConnection, eventSubcription, sp, queueName);
});

builder.Services.AddSingleton<IEventSubcriptionManager, EventSubcriptionManager>();

builder.Services.AddTransient<BookChangeQualityAfterReturnEventHandler>();
builder.Services.AddTransient<BookUpdateQualityAfterBorrowCancelEventHandler>();
builder.Services.AddTransient<BookUpdateQualityAfterBorrowConfirmedEventHandler>();
builder.Services.AddTransient<BorrowWaitForStockConfirmEventHandler>();

//builder.Services.AddAuthentication("Bearer")
//.AddJwtBearer("Bearer", options =>
//{
//    options.Authority = configuration["IdentityUrl"]; //"https://localhost:5678";

//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateAudience = false
//    };
//});

//// adds an authorization policy to make sure the token is for scope 'api1'
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("book", policy =>
//    {
//        policy.RequireAuthenticatedUser();
//        policy.RequireClaim("scope", "book");
//    });
//});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subcribe<BookChangeQualityAfterReturnEvent, BookChangeQualityAfterReturnEventHandler>();
eventBus.Subcribe<BorrowCancelEvent, BookUpdateQualityAfterBorrowCancelEventHandler>();
eventBus.Subcribe<BookUpdateQualityAfterBorrowConfirmedEvent, BookUpdateQualityAfterBorrowConfirmedEventHandler>();
eventBus.Subcribe<BorrowChangeStatusToWaitForStockConfirmEvent, BorrowWaitForStockConfirmEventHandler>();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<BookDBContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<BookSettings>>();
    var log = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();
    dataContext.Database.Migrate();
    
    new SeedData().SeedAsync(dataContext, env, options, log).Wait();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<BookGrpcService>();
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
    endpoints.MapGet("/_proto/", async ctx =>
    {
        ctx.Response.ContentType = "text/plain";
        using var fs = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Proto", "Book.proto"), FileMode.Open, FileAccess.Read);
        using var sr = new StreamReader(fs);
        while (!sr.EndOfStream)
        {
            var line = await sr.ReadLineAsync();
            if (line != "/* >>" || line != "<< */")
            {
                await ctx.Response.WriteAsync(line);
            }
        }
    });
    endpoints.MapGet("/_proto/", async ctx =>
    {
        ctx.Response.ContentType = "text/plain";
        using var fs = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Proto", "protobuf.proto"), FileMode.Open, FileAccess.Read);
        using var sr = new StreamReader(fs);
        while (!sr.EndOfStream)
        {
            var line = await sr.ReadLineAsync();
            if (line != "/* >>" || line != "<< */")
            {
                await ctx.Response.WriteAsync(line);
            }
        }
    });
});

app.Run();

IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", typeof(Program).GetTypeInfo().Assembly.GetName().Name)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.DurableHttpUsingTimeRolledBuffers(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
{
    var grpcPort = config.GetValue("GRPC_PORT", 81);
    var port = config.GetValue("PORT", 80);
    return (port, grpcPort);
}
