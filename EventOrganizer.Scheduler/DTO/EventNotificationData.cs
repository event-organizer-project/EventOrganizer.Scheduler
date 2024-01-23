namespace EventOrganizer.Scheduler.DTO
{
    [Serializable]
    public class EventNotificationData
    {
        public int EventId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public int SubscriptionId { get; set; }

        public string Endpoint { get; set; }

        public string P256DH { get; set; }

        public string Auth { get; set; }
    }
}
