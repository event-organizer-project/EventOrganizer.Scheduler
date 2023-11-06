namespace EventOrganizer.Scheduler.DTO
{
    public class PushServiceOptions
    {
        public string Subject { get; set; } = string.Empty;

        public string PublicKey { get; set; } = string.Empty;

        public string PrivateKey { get; set; } = string.Empty;
    }
}
