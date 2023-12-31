using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.Helpers;
using EventOrganizer.Scheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace EventOrganizer.Scheduler.Controllers
{
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
            scheduler = schedulerFactory.GetScheduler().Result;
            this.eventRepository = eventRepository;
            this.notificationTriggerFactory = notificationTriggerFactory;
            this.logger = logger;
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
        /// <param name="eId">User identifier</param>
        /// <returns></returns>
        [HttpDelete("{eId}/{uId}")]
        public async Task<IActionResult> RemoveEventFromSchedule(int eId, int uId)
        {
            var subscriptionIds = await eventRepository.GetSubscriptionIdsByUserId(uId);

            foreach (var sId in subscriptionIds)
            {
                var triggerKey = TriggerKeyCreationHelper.CreateNotificationTriggerKey(eId, sId);
                await scheduler.UnscheduleJob(triggerKey);
            }

            return Ok();
        }
    }
}