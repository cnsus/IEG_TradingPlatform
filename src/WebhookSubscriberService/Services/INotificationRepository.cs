using WebhookSubscriberService.Models;

namespace WebhookSubscriberService.Services
{
    /// <summary>
    /// Interface fuer den In-Memory Store der empfangenen Webhook-Benachrichtigungen.
    /// </summary>
    public interface INotificationRepository
    {
        IEnumerable<PaymentNotification> GetAll();
        void Add(PaymentNotification notification);
    }
}
