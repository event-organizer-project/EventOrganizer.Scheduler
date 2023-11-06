using EventOrganizer.Scheduler.DTO;
using EventOrganizer.Scheduler.Services;
using Quartz;

namespace EventOrganizer.Scheduler.Jobs
{
    public class NotificationJob : IJob
    {
        public static readonly JobKey Key = new("notification", "job");

        private readonly INotificationService notificationService;

        public NotificationJob(INotificationService notificationService)
        {
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            var eventNotificationData = (EventNotificationData)dataMap[nameof(EventNotificationData)];

            await notificationService.Notify(eventNotificationData);
        }
    }
}
