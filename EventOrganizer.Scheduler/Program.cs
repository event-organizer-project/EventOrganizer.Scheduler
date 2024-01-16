using EventOrganizer.Scheduler;
using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.Services;
using IdentityServer4.AccessTokenValidation;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration.GetValue<bool>("UseCustomSslCertificates"))
{
    var certs = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase)
    {
        ["localhost"] = new X509Certificate2("/app/certificates/localhost.pfx", "password"),
        ["host.docker.internal"] = new X509Certificate2("/app/certificates/host.docker.internal.pfx", "password")
    };

    using (var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser))
    {
        store.Open(OpenFlags.ReadWrite);

        store.Add(certs["localhost"]);
        store.Add(certs["host.docker.internal"]);
    }

    builder.WebHost.ConfigureKestrel(options =>
        options.ConfigureHttpsDefaults(opt =>
        {
            opt.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            opt.ServerCertificateSelector = (connectionContext, name) =>
                name == "host.docker.internal" ? certs["host.docker.internal"] : certs["localhost"]; ;
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

builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
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
