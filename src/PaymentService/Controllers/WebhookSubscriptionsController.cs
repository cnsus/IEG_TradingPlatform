using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookSubscriptionsController : ControllerBase
    {
        private readonly IWebhookService _webhookService;

        public WebhookSubscriptionsController(IWebhookService webhookService)
        {
            _webhookService = webhookService;
        }

        // GET: api/webhooksubscriptions
        // Gibt alle registrierten Webhook-Subscriber zurueck
        [HttpGet]
        public IActionResult GetAll()
        {
            var subscriptions = _webhookService.GetAll().ToList();
            return Ok(subscriptions);
        }

        // POST: api/webhooksubscriptions
        // Registriert einen neuen Webhook-Subscriber mit einer Callback-URL
        [HttpPost]
        public IActionResult Register([FromBody] WebhookSubscription subscription)
        {
            if (string.IsNullOrWhiteSpace(subscription.CallbackUrl))
                return BadRequest(new { Message = "CallbackUrl darf nicht leer sein." });

            var created = _webhookService.Register(subscription);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }

        // DELETE: api/webhooksubscriptions/1
        // Entfernt einen registrierten Webhook-Subscriber
        [HttpDelete("{id}")]
        public IActionResult Unregister(int id)
        {
            _webhookService.Unregister(id);
            return NoContent();
        }
    }
}
