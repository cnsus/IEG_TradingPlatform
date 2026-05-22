using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Content Negotiation: Der Client steuert das Format ueber Accept und Content-Type Header
    // Unterstuetzte Formate: JSON (application/json), XML (application/xml), CSV (text/csv)
    [Produces("application/json", "application/xml", "text/csv")]
    [Consumes("application/json", "application/xml", "text/csv")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _repository;
        private readonly IWebhookService _webhookService;

        public PaymentsController(IPaymentRepository repository, IWebhookService webhookService)
        {
            _repository = repository;
            _webhookService = webhookService;
        }

        // GET: api/payments
        [HttpGet]
        public IActionResult GetAll()
        {
            var payments = _repository.GetAll().ToList();
            return Ok(payments);
        }

        // GET: api/payments/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var payment = _repository.GetById(id);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        // POST: api/payments
        // Aufgabe 7: Nach dem Erstellen eines Payments werden alle registrierten
        // Webhook-Subscriber ueber das neue Payment benachrichtigt.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Payment payment)
        {
            var created = _repository.Add(payment);

            // Webhook-Benachrichtigung an alle Subscriber senden (fire-and-forget Stil)
            await _webhookService.NotifySubscribersAsync(created);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
