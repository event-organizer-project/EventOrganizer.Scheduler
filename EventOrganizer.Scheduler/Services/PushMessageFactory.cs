using EventOrganizer.Scheduler.DTO;
using Lib.Net.Http.WebPush;
using Newtonsoft.Json;

namespace EventOrganizer.Scheduler.Services
{
    public class PushMessageFactory : IPushMessageFactory
    {
        public PushMessage CreatePushMessage(EventNotificationData eventNotificationData)
        {
            var content = JsonConvert.SerializeObject(new 
            {
                title = eventNotificationData.Title,
                body = eventNotificationData.Description,
                eventId = eventNotificationData.EventId
            });

            return new PushMessage(content);
        }
    }
}
