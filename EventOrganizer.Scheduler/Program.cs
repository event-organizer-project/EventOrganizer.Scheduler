using EventOrganizer.Scheduler;
using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuartzSchedule();

builder.Services.AddPushNotificationService(builder.Configuration);

builder.Services.AddTransient<IEventRepository, EventRepository>();
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

var app = builder.Build();

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
