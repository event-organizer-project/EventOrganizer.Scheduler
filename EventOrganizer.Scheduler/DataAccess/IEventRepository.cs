using EventOrganizer.Scheduler.DTO;

namespace EventOrganizer.Scheduler.DataAccess
{
    public interface IEventRepository
    {
        Task<IList<EventNotificationData>> GetTodayEventNotificationsData();

        Task<IList<EventNotificationData>> GetEventNotificationsData(int eventId, int userId);

        Task<IList<int>> GetSubscriptionIdsByUserIds(int[] userIds);
    }
}
