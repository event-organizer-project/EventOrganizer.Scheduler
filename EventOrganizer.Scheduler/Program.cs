using EventOrganizer.Scheduler;
using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.Services;
using EventOrganizer.Utils.Logging;
using EventOrganizer.Utils.WebApplicationExtensions;
using IdentityServer4.AccessTokenValidation;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureCertificates();

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

builder.Services.AddCustomLogger();

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
