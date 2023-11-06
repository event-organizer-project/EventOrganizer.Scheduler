using EventOrganizer.Scheduler.DTO;
using Lib.Net.Http.WebPush;

namespace EventOrganizer.Scheduler.Services
{
    public class PushNotificationService : INotificationService
    {
        private readonly PushServiceClient pushServiceClient;

        private readonly IPushMessageFactory pushMessageFactory;

        public PushNotificationService(PushServiceClient pushServiceClient, IPushMessageFactory pushMessageFactory)
        {
            this.pushServiceClient = pushServiceClient ?? throw new ArgumentNullException(nameof(pushServiceClient));
            this.pushMessageFactory = pushMessageFactory ?? throw new ArgumentNullException(nameof(pushMessageFactory));
        }

        public async Task Notify(EventNotificationData eventNotificationData)
        {
            var subscription = CreatePushSubscription(eventNotificationData);
            var message = pushMessageFactory.CreatePushMessage(eventNotificationData);

            try
            {
                await pushServiceClient.RequestPushMessageDeliveryAsync(subscription, message);
            }
            catch (Exception ex)
            {
                // TO DO: add exception handling
            }
        }

        private static PushSubscription CreatePushSubscription(EventNotificationData eventNotificationData)
        {
            var subscription = new PushSubscription { Endpoint = eventNotificationData.Endpoint };

            subscription.SetKey(PushEncryptionKeyName.Auth, eventNotificationData.Auth);
            subscription.SetKey(PushEncryptionKeyName.P256DH, eventNotificationData.P256DH);

            return subscription;
        }
    }
}
