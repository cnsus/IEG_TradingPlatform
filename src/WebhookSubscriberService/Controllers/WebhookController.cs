using Microsoft.AspNetCore.Mvc;
using WebhookSubscriberService.Models;
using WebhookSubscriberService.Services;

namespace WebhookSubscriberService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly INotificationRepository _repository;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(INotificationRepository repository, ILogger<WebhookController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // POST: api/webhook/payment
        // Dieser Endpunkt wird vom PaymentService aufgerufen, wenn ein neues Payment erstellt wird.
        [HttpPost("payment")]
        public IActionResult ReceivePaymentNotification([FromBody] PaymentNotification notification)
        {
            notification.NotifiedAt = DateTime.Now;
            _repository.Add(notification);

            _logger.LogInformation(
                "Webhook empfangen: Payment #{PaymentId} ({Amount} {Currency}) - Event: {EventType}",
                notification.PaymentId, notification.Amount, notification.Currency, notification.EventType);

            return Ok(new { Message = "Webhook-Benachrichtigung erfolgreich empfangen", PaymentId = notification.PaymentId });
        }

        // GET: api/webhook/notifications
        // Gibt alle bisher empfangenen Webhook-Benachrichtigungen zurueck.
        [HttpGet("notifications")]
        public IActionResult GetAllNotifications()
        {
            var notifications = _repository.GetAll().ToList();
            return Ok(notifications);
        }
    }
}
