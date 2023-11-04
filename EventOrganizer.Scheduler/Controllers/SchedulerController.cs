using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.DTO;
using EventOrganizer.Scheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace EventOrganizer.Scheduler.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class SchedulerController : ControllerBase
    {
        private readonly IScheduler scheduler;

        private readonly IEventRepository eventRepository;

        private readonly INotificationTriggerFactory notificationTriggerFactory;

        public SchedulerController(ISchedulerFactory schedulerFactory, IEventRepository eventRepository,
            INotificationTriggerFactory notificationTriggerFactory)
        {
            scheduler = schedulerFactory.GetScheduler().Result;
            this.eventRepository = eventRepository;
            this.notificationTriggerFactory = notificationTriggerFactory;
        }

        [HttpPost("add-event")]
        public async Task<IActionResult> AddEventToSchedule(ScheduledEvent scheduledEvent)
        {
            var eventData = await eventRepository.GetDetailedEvent(scheduledEvent.EventId, scheduledEvent.UserId);

            if (eventData == null)
            {
                return NotFound();
            }

            var trigger = notificationTriggerFactory.CreateNotificationTrigger(eventData);

            await scheduler.ScheduleJob(trigger);

            return Ok();
        }

        [HttpDelete("remove-event")]
        public IActionResult RemoveEventFromSchedule(ScheduledEvent scheduledEvent)
        {
            // RemoveEventFromSchedule

            return Ok();
        }
    }
}