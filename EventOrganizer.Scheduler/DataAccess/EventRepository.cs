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

        public async Task<DetailedEvent> GetDetailedEvent(int eventId, int userId)
        {
            await using var sqlConnection = connectionFactory.CreateConnection();

            var response = await sqlConnection.QueryFirstOrDefaultAsync<DetailedEvent>(
                @"SELECT e.Id AS EventId, e.Title, e.Description, e.StartTime,
                      e.OwnerId AS UserId, u.FirstName AS UserName, u.Email AS UserEmail
                  FROM eventorganizer.eventmodels e
                  JOIN eventorganizer.eventinvolvement ei ON ei.EventId = e.Id
                  JOIN eventorganizer.Users u ON ei.UserId = u.Id
                  WHERE DATE(e.StartDate) = CURDATE() AND e.Id = @EventId AND u.Id = @UserId",
                new
                {
                    EventId = eventId,
                    UserId = userId
                });

            return response;
        }

        public async Task<IList<DetailedEvent>> GetTodayDetailedEvents()
        {
            await using var sqlConnection = connectionFactory.CreateConnection();

            var response = await sqlConnection.QueryAsync<DetailedEvent>(
                @"SELECT e.Id AS EventId, e.Title, e.Description, e.StartTime,
                      e.OwnerId AS UserId, u.FirstName AS UserName, u.Email AS UserEmail
                  FROM eventorganizer.eventmodels e
                  JOIN eventorganizer.eventinvolvement ei ON ei.EventId = e.Id
                  JOIN eventorganizer.Users u ON ei.UserId = u.Id
                  WHERE DATE(e.StartDate) = CURDATE()");

            if (response == null)
            {
                throw new Exception();
            }

            return response.ToList();
        }
    }
}
