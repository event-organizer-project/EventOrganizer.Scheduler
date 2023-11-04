using EventOrganizer.Scheduler.DTO;
using EventOrganizer.Scheduler.Jobs;
using Quartz;

namespace EventOrganizer.Scheduler.Services
{
    public class DoubleNotificationTriggerFactory : INotificationTriggerFactory
    {
        private const int minutesBetweenRepetitions = 2;

        public ITrigger CreateNotificationTrigger(DetailedEvent detailedEvent)
        {
            var jobDataMap = new JobDataMap
            {
                [nameof(DetailedEvent)] = detailedEvent
            };

            var startAt = detailedEvent.StartTime.Subtract(TimeSpan.FromMinutes(minutesBetweenRepetitions));
            var triggerTime = DateTime.Today.Add(startAt);

            var identityName = $"notification-{detailedEvent.EventId}-{detailedEvent.UserId}";

            var trigger = TriggerBuilder.Create()
                .WithIdentity(identityName, "trigger")
                .ForJob(NotificationJob.Key)
                .UsingJobData(jobDataMap)
                .StartAt(triggerTime)
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
