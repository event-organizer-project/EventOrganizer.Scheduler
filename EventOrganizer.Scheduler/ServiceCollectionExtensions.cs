using EventOrganizer.Scheduler.Jobs;
using Quartz;
using EventOrganizer.Scheduler.DTO;
using EventOrganizer.Scheduler.Services;
using System.Configuration;

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

        public static IServiceCollection AddPushNotificationService(this IServiceCollection services, IConfiguration configuration)
        {
            var pushOptions = configuration.GetSection(nameof(PushServiceOptions)).Get<PushServiceOptions>()
                ?? throw new ConfigurationErrorsException($"Section {nameof(PushServiceOptions)} not configured");

            services.AddMemoryCache();
            services.AddMemoryVapidTokenCache();
            services.AddPushServiceClient(options =>
            {
                options.Subject = pushOptions.Subject;
                options.PublicKey = pushOptions.PublicKey;
                options.PrivateKey = pushOptions.PrivateKey;
            });

            services.AddTransient<INotificationService, PushNotificationService>();

            return services;
        }
    }
}
