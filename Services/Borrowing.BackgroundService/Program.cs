using BookOnline.Borrowing.BackgroundTask;
using RabbitMQ.Client;
using RabbitMQEventBus;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = GetConfiguration();

// Add services to the container.
//Create seri log
Log.Logger = CreateSerilogLogger(configuration);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    var queueName = "BackgroundTaskAPI";//configuration["QueueName"];
    var rabbitMQConnection = sp.GetRequiredService<IRabbitMQConncection>();

    var eventSubcription = sp.GetRequiredService<IEventSubcriptionManager>();

    return new RabbitMQEventBus.RabbitMQEventBus(rabbitMQConnection, eventSubcription, sp, queueName);
});

builder.Services.AddSingleton<IEventSubcriptionManager, EventSubcriptionManager>();

builder.Services.Configure<BackgroundTaskSettings>(configuration)
    .AddHostedService<BorrowingServiceManager>();

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
