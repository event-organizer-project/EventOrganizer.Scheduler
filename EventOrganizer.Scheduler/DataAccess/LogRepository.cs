using Dapper;

namespace EventOrganizer.Scheduler.DataAccess
{
    public class LogRepository : ILogRepository
    {
        private readonly ISqlConnectionFactory connectionFactory;

        public LogRepository(ISqlConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task SaveLog(LogRecord logRecord)
        {
            await using var sqlConnection = connectionFactory.CreateConnection();

            var response = sqlConnection.Execute(
            @"INSERT INTO EventOrganizer.LogRecords(LogLevel, StackTrace, Message, ExceptionMessage, AdditionalInfo, CallerName, CreatedAt, Application)
                   VALUES(@LogLevel, @StackTrace, @Message, @ExceptionMessage, @AdditionalInfo, @CallerName, @CreatedAt, @Application)",
            logRecord);
        }
    }
}
