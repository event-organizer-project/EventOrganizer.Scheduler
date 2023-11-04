using EventOrganizer.Scheduler.DTO;

namespace EventOrganizer.Scheduler.DataAccess
{
    public interface IEventRepository
    {
        Task<IList<DetailedEvent>> GetTodayDetailedEvents();

        Task<DetailedEvent?> GetDetailedEvent(int eventId, int userId);
    }
}
