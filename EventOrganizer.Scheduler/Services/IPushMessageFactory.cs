using EventOrganizer.Scheduler.DTO;
using Lib.Net.Http.WebPush;

namespace EventOrganizer.Scheduler.Services
{
    public interface IPushMessageFactory
    {
        PushMessage CreatePushMessage(EventNotificationData eventNotificationData);
    }
}
