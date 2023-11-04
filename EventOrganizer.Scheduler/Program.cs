using EventOrganizer.Scheduler;
using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuartzSchedule();

builder.Services.AddTransient<INotificationService, NotificationServiceMock>();
builder.Services.AddTransient<IEventRepository, EventRepository>();
builder.Services.AddTransient<ISqlConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddTransient<INotificationTriggerFactory, DoubleNotificationTriggerFactory>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
