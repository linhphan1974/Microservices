using BookOnline.Ordering.Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order.Api.Infrastructure.Data;
using RabbitMQ.Client;
using RabbitMQEventBus;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
IConfiguration configuration = GetConfiguration();

// Add services to the container.
//Create seri log
Log.Logger = CreateSerilogLogger(configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register database context
builder.Services.AddEntityFrameworkSqlServer()
    .AddDbContext<OrderingDBContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("OrderDB"),
                                        sqlServerOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                        });
    });

//RabbitMQ
ConnectionFactory factory = new ConnectionFactory();
factory.HostName = configuration["EventHost"];//"rabbitmq";//
factory.Port = 5672;
factory.UserName = "guest";
factory.Password = "guest";
factory.DispatchConsumersAsync = true;

builder.Services.AddSingleton<IEventSubcriptionManager, EventSubcriptionManager>();

builder.Services.AddSingleton<IRabbitMQConncection, RabbitMQConnection>(sp =>
{
    return new RabbitMQConnection(factory, 3);
});


builder.Services.AddSingleton<IEventBus, RabbitMQEventBus.RabbitMQEventBus>(sp =>
{
    var queueName = "Test_Event";//configuration["QueueName"];
    var rabbitMQConnection = sp.GetRequiredService<IRabbitMQConncection>();

    var eventSubcription = sp.GetRequiredService<IEventSubcriptionManager>();

    return new RabbitMQEventBus.RabbitMQEventBus(rabbitMQConnection, eventSubcription, sp, queueName);
});

// register mediatr
builder.Services.AddMediatR(typeof(Program).Assembly);

builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://localhost:5678";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

// adds an authorization policy to make sure the token
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("order", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "order");
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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