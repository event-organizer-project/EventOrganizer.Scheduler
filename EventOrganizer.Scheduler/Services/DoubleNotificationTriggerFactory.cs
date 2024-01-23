using EventOrganizer.Scheduler.DTO;
using EventOrganizer.Scheduler.Helpers;
using EventOrganizer.Scheduler.Jobs;
using Quartz;

namespace EventOrganizer.Scheduler.Services
{
    public class DoubleNotificationTriggerFactory : INotificationTriggerFactory
    {
        private const int minutesBetweenRepetitions = 2;

        public ITrigger CreateNotificationTrigger(EventNotificationData eventNotificationData)
        {
            var jobDataMap = new JobDataMap
            {
                [nameof(EventNotificationData)] = eventNotificationData
            };

            var startAt = eventNotificationData.StartDate.Subtract(TimeSpan.FromMinutes(minutesBetweenRepetitions))
                .ToLocalTime();

            var identity = TriggerKeyCreationHelper.CreateNotificationTriggerKey(eventNotificationData);

            var trigger = TriggerBuilder.Create()
                .WithIdentity(identity)
                .ForJob(NotificationJob.Key)
                .UsingJobData(jobDataMap)
                .StartAt(startAt)
                .WithSimpleSchedule(o =>
                {
                    o.WithRepeatCount(1)
                    .WithInterval(TimeSpan.FromMinutes(minutesBetweenRepetitions));
                })
                .Build();

            return trigger;
        }
    }
}
