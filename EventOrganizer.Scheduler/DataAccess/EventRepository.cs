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
                @"SELECT e.Id AS EventId, e.Title, e.Description, e.StartDate,
                      s.Id AS SubscriptionId, s.Endpoint AS Endpoint, s.P256DH AS P256DH, s.Auth AS Auth
                  FROM EventOrganizer.EventModels e
                  JOIN EventOrganizer.EventInvolvement ei ON ei.EventId = e.Id
                  JOIN EventOrganizer.Subscriptions s ON ei.UserId = s.UserId
                  WHERE e.Id = @EventId AND ei.UserId = @UserId AND e.StartDate BETWEEN @CurrentTime AND @EndTime;",
                new
                {
                    EventId = eventId,
                    UserId = userId,
                    CurrentTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.Date.AddDays(1),
                });

            return response.ToList();
        }

        public async Task<IList<EventNotificationData>> GetTodayEventNotificationsData()
        {
            await using var sqlConnection = connectionFactory.CreateConnection();

            var response = await sqlConnection.QueryAsync<EventNotificationData>(
                @"SELECT e.Id AS EventId, e.Title, e.Description, e.StartDate,
                      s.Id AS SubscriptionId, s.Endpoint AS Endpoint, s.P256DH AS P256DH, s.Auth AS Auth
                  FROM EventOrganizer.EventModels e
                  JOIN EventOrganizer.EventInvolvement ei ON ei.EventId = e.Id
                  JOIN EventOrganizer.Subscriptions s ON ei.UserId = s.UserId
                  WHERE e.StartDate BETWEEN @CurrentTime AND @EndTime;",
                new
                {
                    CurrentTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.Date.AddDays(1),
                });

            return response.ToList();
        }

        public async Task<IList<int>> GetSubscriptionIdsByUserIds(int[] userIds)
        {
            await using var sqlConnection = connectionFactory.CreateConnection();

            var response = await sqlConnection.QueryAsync<int>(
                @"SELECT s.Id FROM EventOrganizer.Subscriptions s
                  WHERE s.UserId IN @UserIds",
                new { UserIds = userIds });

            return response.ToList();
        }
    }
}
