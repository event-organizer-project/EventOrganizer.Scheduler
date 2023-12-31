using EventOrganizer.Scheduler;
using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.Services;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
        options.ConfigureHttpsDefaults(opt =>
        {
            opt.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            opt.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
            opt.ServerCertificate = new X509Certificate2("aspnetapp.pfx", "password");
        }));
}

builder.Services.AddQuartzSchedule();

builder.Services.AddPushNotificationService(builder.Configuration);

builder.Services.AddTransient<IEventRepository, EventRepository>();
builder.Services.AddTransient<ILogRepository, LogRepository>();
builder.Services.AddTransient<ISqlConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddTransient<INotificationTriggerFactory, DoubleNotificationTriggerFactory>();
builder.Services.AddTransient<IPushMessageFactory, PushMessageFactory>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Bearer")
    .AddIdentityServerAuthentication("Bearer", options =>
    {
        options.ApiName = "scheduler_api";
        options.Authority = builder.Configuration["Authority"];
    });

builder.Services.AddSingleton<ILoggerProvider, CustomLoggerProvider>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) => 
{
    if (context.Request.Path == "/" || !context.Request.Path.HasValue)
        await context.Response.WriteAsync("Event Organizer Scheduler Service has started.");
    else
        await next.Invoke();
});

app.Run();
