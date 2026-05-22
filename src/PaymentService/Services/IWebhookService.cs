using PaymentService.Models;

namespace PaymentService.Services
{
    /// <summary>
    /// Interface fuer die Verwaltung von Webhook-Subscriptions.
    /// </summary>
    public interface IWebhookService
    {
        IEnumerable<WebhookSubscription> GetAll();
        WebhookSubscription Register(WebhookSubscription subscription);
        void Unregister(int id);
        Task NotifySubscribersAsync(Payment payment);
    }
}
