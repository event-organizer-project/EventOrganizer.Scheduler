namespace EventOrganizer.Scheduler.DataAccess
{
    public interface ILogRepository
    {
        Task SaveLog(LogRecord logRecord);
    }
}
