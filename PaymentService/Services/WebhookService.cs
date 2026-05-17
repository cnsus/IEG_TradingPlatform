using System.Text;
using System.Text.Json;
using PaymentService.Models;

namespace PaymentService.Services
{
    /// <summary>
    /// Verwaltet Webhook-Subscriptions und benachrichtigt alle registrierten Subscriber
    /// via HTTP POST, wenn ein neues Payment erstellt wird.
    /// </summary>
    public class WebhookService : IWebhookService
    {
        private static readonly List<WebhookSubscription> _subscriptions = new();
        private static int _nextId = 1;
        private readonly ILogger<WebhookService> _logger;

        public WebhookService(ILogger<WebhookService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<WebhookSubscription> GetAll() => _subscriptions;

        public WebhookSubscription Register(WebhookSubscription subscription)
        {
            subscription.Id = _nextId++;
            subscription.RegisteredAt = DateTime.Now;
            subscription.IsActive = true;
            _subscriptions.Add(subscription);

            _logger.LogInformation("Neuer Webhook-Subscriber registriert: {CallbackUrl} fuer Event '{EventType}'",
                subscription.CallbackUrl, subscription.EventType);

            return subscription;
        }

        public void Unregister(int id)
        {
            var subscription = _subscriptions.FirstOrDefault(s => s.Id == id);
            if (subscription != null)
            {
                _subscriptions.Remove(subscription);
                _logger.LogInformation("Webhook-Subscriber entfernt: {CallbackUrl}", subscription.CallbackUrl);
            }
        }

        /// <summary>
        /// Benachrichtigt alle aktiven Subscriber ueber ein neues Payment via HTTP POST.
        /// Fehlerhafte Zustellungen werden geloggt, blockieren aber nicht den Hauptprozess.
        /// </summary>
        public async Task NotifySubscribersAsync(Payment payment)
        {
            var activeSubscriptions = _subscriptions.Where(s => s.IsActive && s.EventType == "payment.created").ToList();

            if (!activeSubscriptions.Any())
            {
                _logger.LogInformation("Keine aktiven Webhook-Subscriber vorhanden. Ueberspringe Benachrichtigung.");
                return;
            }

            var payload = new
            {
                PaymentId = payment.Id,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Description = payment.Description,
                PaymentMethod = payment.PaymentMethod,
                CreatedAt = payment.CreatedAt,
                EventType = "payment.created"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Selbst-signierte Zertifikate fuer Entwicklung akzeptieren
            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using var httpClient = new HttpClient(handler);

            foreach (var subscription in activeSubscriptions)
            {
                try
                {
                    var response = await httpClient.PostAsync(subscription.CallbackUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Webhook erfolgreich zugestellt an: {CallbackUrl} (Payment #{PaymentId})",
                            subscription.CallbackUrl, payment.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Webhook-Zustellung fehlgeschlagen an: {CallbackUrl} - Status: {StatusCode}",
                            subscription.CallbackUrl, response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fehler bei Webhook-Zustellung an: {CallbackUrl}", subscription.CallbackUrl);
                }
            }
        }
    }
}
