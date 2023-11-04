using EventOrganizer.Scheduler.DTO;
using Quartz;

namespace EventOrganizer.Scheduler.Services
{
    public interface INotificationTriggerFactory
    {
        ITrigger CreateNotificationTrigger(DetailedEvent detailedEvent);
    }
}
