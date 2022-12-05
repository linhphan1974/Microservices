using BookOnline.BorrowNotification.Api;
using BookOnline.BorrowNotification.Api.Application.EventHandlers;
using BookOnline.BorrowNotification.Api.Application.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using RabbitMQEventBus;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = GetConfiguration();
// Add services to the container.
IdentityModelEventSource.ShowPII = true;  // Caution! Do NOT use in production: https://aka.ms/IdentityModel/PII

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("cors",
        builder =>
        {
            builder.WithOrigins("null")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

//Create seri log
Log.Logger = CreateSerilogLogger(configuration);

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
    var queueName = "NotificationAPI";//configuration["QueueName"];
    var rabbitMQConnection = sp.GetRequiredService<IRabbitMQConncection>();

    var eventSubcription = sp.GetRequiredService<IEventSubcriptionManager>();

    return new RabbitMQEventBus.RabbitMQEventBus(rabbitMQConnection, eventSubcription, sp, queueName);
});

builder.Services.AddSingleton<IEventSubcriptionManager, EventSubcriptionManager>();

builder.Services.AddTransient<BorrowChangeStatusToSubmittedEventHandler>();
builder.Services.AddTransient<BorrowChangeStatusToConfirmedEventHandler>();
builder.Services.AddTransient<BorrowChangeStatusToRejectedEventHandler>();
builder.Services.AddTransient<BorrowChangeStatusToCancelledEventHandler>();
builder.Services.AddTransient<BorrowChangeStatusToWaitForPickupEventHandler>();
builder.Services.AddTransient<BorrowChangeStatusToWaitForShipEventHandler>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = configuration["IdentityUrl"];//"https://localhost:5678";
    options.Audience = "signalr";
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hub/notificationhub")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// adds an authorization policy to make sure the token
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("signalr", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "signalr");
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// register event handler
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subcribe<BorrowChangeStatusToSubmittedEvent, BorrowChangeStatusToSubmittedEventHandler>();
eventBus.Subcribe<BorrowChangeStatusToConfirmedEvent, BorrowChangeStatusToConfirmedEventHandler>();
eventBus.Subcribe<BorrowChangeStatusToRejectedEvent, BorrowChangeStatusToRejectedEventHandler>();
eventBus.Subcribe<BorrowChangeStatusToCancelledEvent, BorrowChangeStatusToCancelledEventHandler>();
eventBus.Subcribe<BorrowChangeStatusToWaitForPickupEvent, BorrowChangeStatusToWaitForPickupEventHandler>();

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("cors");
app.MapHub<NotificationHub>("/hub/notificationhub");
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
