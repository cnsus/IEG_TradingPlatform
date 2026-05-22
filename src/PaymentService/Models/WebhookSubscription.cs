namespace PaymentService.Models
{
    /// <summary>
    /// Repraesentiert einen registrierten Webhook-Subscriber.
    /// </summary>
    public class WebhookSubscription
    {
        public int Id { get; set; }
        public string CallbackUrl { get; set; } = string.Empty;
        public string EventType { get; set; } = "payment.created";
        public DateTime RegisteredAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
