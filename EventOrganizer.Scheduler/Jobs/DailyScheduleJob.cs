using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.Services;
using Quartz;

namespace EventOrganizer.Scheduler.Jobs
{
    public class DailyScheduleJob : IJob
    {
        public static readonly JobKey Key = new("daily-schedule", "job");

        private readonly IScheduler scheduler;

        private readonly IEventRepository eventRepository;

        private readonly INotificationTriggerFactory notificationTriggerFactory;

        public DailyScheduleJob(ISchedulerFactory schedulerFactory, IEventRepository eventRepository,
            INotificationTriggerFactory notificationTriggerFactory)
        {
            scheduler = schedulerFactory.GetScheduler().Result;
            this.eventRepository = eventRepository;
            this.notificationTriggerFactory = notificationTriggerFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var events = await eventRepository.GetTodayDetailedEvents();

            var triggers = events.Select(x => notificationTriggerFactory.CreateNotificationTrigger(x));

            foreach (var trigger in triggers)
            {
                await scheduler.ScheduleJob(trigger);
            }
        }
    }
}