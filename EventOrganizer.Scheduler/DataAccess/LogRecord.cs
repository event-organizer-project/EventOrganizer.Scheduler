namespace EventOrganizer.Scheduler.DataAccess
{
    public class LogRecord
    {
        public int Id { get; set; }

        public string LogLevel { get; set; }

        public string? StackTrace { get; set; }

        public string? Message { get; set; }

        public string? ExceptionMessage { get; set; }

        public string? AdditionalInfo { get; set; }

        public string CallerName { get; set; }

        public string? Application { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
