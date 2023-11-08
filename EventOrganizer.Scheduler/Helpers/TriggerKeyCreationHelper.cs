using EventOrganizer.Scheduler.DTO;
using Quartz;

namespace EventOrganizer.Scheduler.Helpers
{
    public class TriggerKeyCreationHelper
    {
        public static TriggerKey CreateNotificationTriggerKey(EventNotificationData eventNotificationData)
        {
            return CreateNotificationTriggerKey(eventNotificationData.EventId, eventNotificationData.SubscriptionId);
        }

        public static TriggerKey CreateNotificationTriggerKey(int eventId, int subscriptionId)
        {
            const string group = "trigger";

            var name = $"notification-{eventId}-{subscriptionId}";

            return new TriggerKey(name, group);
        }
    }
}
