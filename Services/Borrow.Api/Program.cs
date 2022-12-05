using Autofac;
using Autofac.Extensions.DependencyInjection;
using BookOnline.Borrowing.Api;
using BookOnline.Borrowing.Api.Behaviors;
using BookOnline.Borrowing.Api.GrpcService;
using BookOnline.Borrowing.Api.Infrastucture.AutofacModules;
using BookOnline.Borrowing.Api.Infrastucture.Commands;
using BookOnline.Borrowing.Api.Infrastucture.EventHandlers;
using BookOnline.Borrowing.Api.Infrastucture.Events;
using BookOnline.Borrowing.Api.Infrastucture.Repositories;
using BookOnline.EventLogService;
using BookOnline.EventLogService.Services;
using BookOnline.EventServiceLog.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.AutofacModules;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using RabbitMQEventBus;
using Serilog;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = GetConfiguration();

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

//builder.Services.AddHttpClient("Borrowing.Api")
//        .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
//        {
//            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
//        });

IdentityModelEventSource.ShowPII = true;  // Caution! Do NOT use in production: https://aka.ms/IdentityModel/PII


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions();

builder.Services.Configure<Settings>(configuration);

//Add Grpc
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;

});


//Register database context
builder.Services.AddDbContext<BorrowingDBContext>(options =>
    {
        options.UseSqlServer(configuration["ConnectionString"].ToString(),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
    },
        ServiceLifetime.Singleton
    );
builder.Services.AddDbContext<EventLogContext>(options =>
    {
        options.UseSqlServer(configuration["ConnectionString"].ToString(),
                                        sqlServerOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                        });
    });

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();


builder.Services.AddTransient<Func<DbConnection, IApplicationEventLogService>>(
    sp => (DbConnection c) => new ApplicationEventLogService(c));

builder.Services.AddTransient<IBorrowingApplicationEventService, BorrowingApplicationEventService>();


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
    var queueName = "BorrowAPI";//configuration["QueueName"];
    var rabbitMQConnection = sp.GetRequiredService<IRabbitMQConncection>();

    var eventSubcription = sp.GetRequiredService<IEventSubcriptionManager>();

    return new RabbitMQEventBus.RabbitMQEventBus(rabbitMQConnection, eventSubcription, sp, queueName);
});


builder.Services.AddSingleton<IEventSubcriptionManager, EventSubcriptionManager>();

builder.Services.AddTransient<BorrowProcessStartedEventHandler>();
builder.Services.AddTransient<BorrowStockConfirmedEventHandler>();
builder.Services.AddTransient<BorrowStockRejectEventHandler>();
builder.Services.AddTransient<CheckoutBorrowEventHandler>();
builder.Services.AddTransient<ApplicationEvent>();

//register mediatr
builder.Services.AddTransient<IBorrowRepository, BorrowRepository>();
builder.Services.AddTransient<IMemberRepository, MemberRepository>();

builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = configuration["IdentityUrl"];// "https://localhost:5678";
    options.Audience = "borrow";
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

// adds an authorization policy to make sure the token
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("borrow", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "borrow");
    });
});

//var container = new ContainerBuilder();
//container.Populate(builder.Services);

//container.RegisterModule(new MediatorModule());
//container.RegisterModule(new ApplicationModule(configuration["ConnectionString"]));

//new AutofacServiceProvider(container.Build());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");


using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<BorrowingDBContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<EventLogContext>().Database.Migrate();
}

// register event handler
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subcribe<CheckoutBorrowEvent, CheckoutBorrowEventHandler>();
eventBus.Subcribe<BorrowProcessStartedEvent, BorrowProcessStartedEventHandler>();
eventBus.Subcribe<BorrowStockConfirmedEvent, BorrowStockConfirmedEventHandler>();
eventBus.Subcribe<BorrowStockRejectEvent, BorrowStockRejectEventHandler>();

app.UseRouting();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<BorrowGrpcService>();
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
    endpoints.MapGet("/_proto/", async ctx =>
    {
        ctx.Response.ContentType = "text/plain";
        using var fs = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Proto", "Borrow.proto"), FileMode.Open, FileAccess.Read);
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
