namespace WebhookSubscriberService.Models
{
    /// <summary>
    /// Repraesentiert eine empfangene Webhook-Benachrichtigung ueber eine neue Zahlung.
    /// </summary>
    public class PaymentNotification
    {
        public int PaymentId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime NotifiedAt { get; set; }
    }
}
