namespace EventOrganizer.Scheduler.DTO
{
    [Serializable]
    public class DetailedEvent
    {
        public int EventId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public TimeSpan StartTime { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }
    }
}
