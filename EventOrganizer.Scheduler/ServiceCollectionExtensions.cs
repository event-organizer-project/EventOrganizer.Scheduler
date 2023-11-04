using EventOrganizer.Scheduler.Jobs;
using Quartz;

namespace EventOrganizer.Scheduler
{
    public static class ServiceCollectionExtensions
    {
        public static void AddQuartzSchedule(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.AddJob<DailyScheduleJob>(opts => opts.WithIdentity(DailyScheduleJob.Key));
                q.AddJob<NotificationJob>(opts => opts.WithIdentity(NotificationJob.Key).StoreDurably());

                q.AddTrigger(opts => opts
                    .ForJob(DailyScheduleJob.Key)
                    .WithIdentity("DailyScheduleJob-trigger")
                    /*.WithCronSchedule("0/5 * * * * ?")*/); // run every 5 seconds

            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}
