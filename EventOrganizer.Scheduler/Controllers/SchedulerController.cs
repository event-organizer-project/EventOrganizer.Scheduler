using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.Helpers;
using EventOrganizer.Scheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace EventOrganizer.Scheduler.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SchedulerController : ControllerBase
    {
        private readonly IScheduler scheduler;

        private readonly IEventRepository eventRepository;

        private readonly INotificationTriggerFactory notificationTriggerFactory;

        private readonly ILogger<SchedulerController> logger;

        public SchedulerController(ISchedulerFactory schedulerFactory, IEventRepository eventRepository,
            INotificationTriggerFactory notificationTriggerFactory, ILogger<SchedulerController> logger)
        {
            scheduler = schedulerFactory?.GetScheduler()?.Result ?? throw new ArgumentNullException(nameof(schedulerFactory));
            this.eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            this.notificationTriggerFactory = notificationTriggerFactory ?? throw new ArgumentNullException(nameof(notificationTriggerFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Schedule event notification for user by ids.
        /// </summary>
        /// <param name="eId">Event identifier</param>
        /// <param name="eId">User identifier</param>
        /// <returns></returns>
        [HttpPost("{eId}/{uId}")]
        public async Task<IActionResult> AddEventToSchedule(int eId, int uId)
        {
            logger.LogInformation($"Execute AddEventToSchedule(int eId={eId}, int uId={uId})");

            var eventNotifications = await eventRepository.GetEventNotificationsData(eId, uId);

            foreach (var eventNotification in eventNotifications)
            {
                var trigger = notificationTriggerFactory.CreateNotificationTrigger(eventNotification);
                await scheduler.ScheduleJob(trigger);
            }

            return Ok();
        }

        /// <summary>
        /// Unchedule event notification for user by ids.
        /// </summary>
        /// <param name="eId">Event identifier</param>
        /// <param name="uIds">User identifiers</param>
        /// <returns></returns>
        [HttpDelete("{eId}/{uIds}")]
        public async Task<IActionResult> RemoveEventFromSchedule(int eId, string uIds)
        {
            var userIds = uIds.Split(',').Select(x => int.Parse(x));

            var subscriptionIds = await eventRepository.GetSubscriptionIdsByUserId(userIds.First());

            foreach (var sId in subscriptionIds)
            {
                var triggerKey = TriggerKeyCreationHelper.CreateNotificationTriggerKey(eId, sId);
                await scheduler.UnscheduleJob(triggerKey);
            }

            return Ok();
        }
    }
}