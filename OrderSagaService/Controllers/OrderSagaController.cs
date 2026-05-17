using Microsoft.AspNetCore.Mvc;
using OrderSagaService.Models;
using OrderSagaService.Services;

namespace OrderSagaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderSagaController : ControllerBase
    {
        private readonly IOrderSagaOrchestrator _orchestrator;
        private readonly IOrderRepository _repository;

        public OrderSagaController(IOrderSagaOrchestrator orchestrator, IOrderRepository repository)
        {
            _orchestrator = orchestrator;
            _repository = repository;
        }

        // POST: api/ordersaga/place
        // Startet eine neue Bestell-Saga, die Produktverfuegbarkeit prueft
        // und eine Zahlung erstellt. Bei Fehler wird kompensiert.
        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ProductName))
                return BadRequest(new { Message = "ProductName darf nicht leer sein." });

            if (request.Amount <= 0)
                return BadRequest(new { Message = "Amount muss groesser als 0 sein." });

            var (order, steps) = await _orchestrator.PlaceOrderAsync(request);

            var result = new
            {
                Order = order,
                SagaSteps = steps,
                SagaOutcome = order.Status == OrderStatus.Confirmed ? "ERFOLGREICH" : "FEHLGESCHLAGEN (Kompensation durchgefuehrt)"
            };

            if (order.Status == OrderStatus.Confirmed)
                return Ok(result);

            // Saga fehlgeschlagen, aber Kompensation wurde durchgefuehrt
            return StatusCode(409, result);
        }

        // GET: api/ordersaga/orders
        // Gibt alle Bestellungen und deren Status zurueck
        [HttpGet("orders")]
        public IActionResult GetAllOrders()
        {
            var orders = _repository.GetAll().ToList();
            return Ok(orders);
        }

        // GET: api/ordersaga/orders/1
        // Gibt eine einzelne Bestellung anhand der ID zurueck
        [HttpGet("orders/{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _repository.GetById(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}
