using EventOrganizer.Scheduler.DTO;

namespace EventOrganizer.Scheduler.Services
{
    public class NotificationServiceMock : INotificationService
    {
        public NotificationServiceMock()
        {
        }

        public async Task Notify(DetailedEvent scheduledEvent)
        {
            var notification = $"Title: {scheduledEvent.Title}\nUser: {scheduledEvent.UserName}\nStarts at: {scheduledEvent.StartTime}\nCurrent Time: {DateTime.Now}\n";

            await Console.Out.WriteLineAsync(notification);
        }
    }
}
