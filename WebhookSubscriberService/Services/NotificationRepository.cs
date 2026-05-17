using WebhookSubscriberService.Models;

namespace WebhookSubscriberService.Services
{
    /// <summary>
    /// Lokaler Datastore dieses Microservices (Decentralized Data Management).
    /// Speichert alle empfangenen Webhook-Benachrichtigungen in-memory.
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        private static readonly List<PaymentNotification> _notifications = new();

        public IEnumerable<PaymentNotification> GetAll() => _notifications;

        public void Add(PaymentNotification notification)
        {
            _notifications.Add(notification);
        }
    }
}
