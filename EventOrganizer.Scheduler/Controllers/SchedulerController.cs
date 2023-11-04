using EventOrganizer.Scheduler.DataAccess;
using EventOrganizer.Scheduler.DTO;
using EventOrganizer.Scheduler.Services;
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