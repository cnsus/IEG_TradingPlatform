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

        public PaymentsController(IPaymentRepository repository)
        {
            _repository = repository;
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
        [HttpPost]
        public IActionResult Create([FromBody] Payment payment)
        {
            var created = _repository.Add(payment);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
