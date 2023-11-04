using EventOrganizer.Scheduler.DTO;

namespace EventOrganizer.Scheduler.Services
{
    public interface INotificationService
    {
        Task Notify(DetailedEvent scheduledEvent);
    }
}
