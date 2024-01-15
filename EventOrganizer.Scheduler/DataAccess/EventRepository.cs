using Dapper;
using EventOrganizer.Scheduler.DTO;

namespace EventOrganizer.Scheduler.DataAccess
{
    public class EventRepository : IEventRepository
    {
        private readonly ISqlConnectionFactory connectionFactory;

        public EventRepository(ISqlConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<IList<EventNotificationData>> GetEventNotificationsData(int eventId, int userId)
        {
            await using var sqlConnection = connectionFactory.CreateConnection();

            var response = await sqlConnection.QueryAsync<EventNotificationData>(
                @"SELECT e.Id AS EventId, e.Title, e.Description, e.StartTime,
                      s.Id AS SubscriptionId, s.Endpoint AS Endpoint, s.P256DH AS P256DH, s.Auth AS Auth
                  FROM eventorganizer.eventmodels e
                  JOIN eventorganizer.eventinvolvement ei ON ei.EventId = e.Id
                  JOIN eventorganizer.subscriptions s ON ei.UserId = s.UserId
                  WHERE DATE(e.StartDate) = CURDATE() AND e.Id = @EventId AND ei.UserId = @UserId",
                new
                {
                    EventId = eventId,
                    UserId = userId
                });

            return response.ToList();
        }

        public async Task<IList<EventNotificationData>> GetTodayEventNotificationsData()
        {
            await using var sqlConnection = connectionFactory.CreateConnection();

            var response = await sqlConnection.QueryAsync<EventNotificationData>(
                @"SELECT e.Id AS EventId, e.Title, e.Description, e.StartTime,
                      s.Id AS SubscriptionId, s.Endpoint AS Endpoint, s.P256DH AS P256DH, s.Auth AS Auth
                  FROM eventorganizer.eventmodels e
                  JOIN eventorganizer.eventinvolvement ei ON ei.EventId = e.Id
                  JOIN eventorganizer.subscriptions s ON ei.UserId = s.UserId
                  WHERE DATE(e.StartDate) = CURDATE()");

            return response.ToList();
        }

        public async Task<IList<int>> GetSubscriptionIdsByUserIds(int[] userIds)
        {
            await using var sqlConnection = connectionFactory.CreateConnection();

            var response = await sqlConnection.QueryAsync<int>(
                @"SELECT s.Id FROM eventorganizer.subscriptions s
                  WHERE s.UserId IN @UserIds",
                new { UserIds = userIds });

            return response.ToList();
        }
    }
}
