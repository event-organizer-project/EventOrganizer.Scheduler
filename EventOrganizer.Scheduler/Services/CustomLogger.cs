using EventOrganizer.Scheduler.DataAccess;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace EventOrganizer.Scheduler.Services
{
    public class CustomLogger : ILogger
    {
        private readonly IServiceScopeFactory serviceProvider;

        private readonly string categoryName;

        public CustomLogger(IServiceScopeFactory serviceProvider, string categoryName)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <summary>  
        /// Whether to log the entry.  
        /// </summary>  
        /// <param name="logLevel"></param>  
        /// <returns></returns>  
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }


        /// <summary>  
        /// Used to log the entry.  
        /// </summary>  
        /// <typeparam name="TState"></typeparam>  
        /// <param name="logLevel">An instance of <see cref="LogLevel"/>.</param>  
        /// <param name="eventId">The event's ID. An instance of <see cref="EventId"/>.</param>  
        /// <param name="state">The event's state.</param>  
        /// <param name="exception">The event's exception. An instance of <see cref="Exception" /></param>  
        /// <param name="formatter">A delegate that formats </param>  
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            using var scope = serviceProvider.CreateScope();

            var logRepository = scope.ServiceProvider.GetRequiredService<ILogRepository>();

            var record = new LogRecord();

            var values = new JObject
            {
                ["ThreadId"] = Environment.CurrentManagedThreadId, // Get the current thread ID to use in the log file.
                ["EventId"] = eventId.Id
            };

            if (!string.IsNullOrWhiteSpace(logLevel.ToString()))
            {
                record.LogLevel = logLevel.ToString();
            }
            if (!string.IsNullOrWhiteSpace(eventId.Name))
            {
                values["EventName"] = eventId.Name;
            }
            if (!string.IsNullOrWhiteSpace(formatter(state, exception)))
            {
                record.Message = formatter(state, exception);
            }
            if (!string.IsNullOrWhiteSpace(exception?.Message))
            {
                record.ExceptionMessage = exception.Message;
            }
            if (!string.IsNullOrWhiteSpace(exception?.StackTrace))
            {
                record.StackTrace = exception.StackTrace;
            }
            if (!string.IsNullOrWhiteSpace(exception?.Source))
            {
                values["ExceptionSource"] = exception.Source;
            }

            record.AdditionalInfo = JsonConvert.SerializeObject(values, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None
            }).ToString();

            record.CallerName = categoryName;
            record.CreatedAt = DateTime.UtcNow;
            record.Application = "scheduler";

            logRepository.SaveLog(record);
        }
    }
}
