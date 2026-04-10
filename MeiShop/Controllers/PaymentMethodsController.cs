using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeiShop.Services;

namespace MeiShop.Controllers
{
    /// <summary>
    /// API Gateway Controller fuer Payment-Methoden.
    /// Ruft den IEGEasyCreditcardService ueber den ResilientServiceCaller auf,
    /// der Round-Robin Load Balancing, Retry (4x pro Instanz) und Failover implementiert.
    /// Fehler werden ueber gRPC an den zentralen LoggingService gesendet.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        //https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        private readonly ILogger<PaymentMethodsController> _logger;
        private readonly ResilientServiceCaller _serviceCaller;

        public PaymentMethodsController(ILogger<PaymentMethodsController> logger, ResilientServiceCaller serviceCaller)
        {
            _logger = logger;
            _serviceCaller = serviceCaller;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            _logger.LogInformation("GET /api/paymentmethods aufgerufen – Starte resilienten Aufruf...");

            var response = await _serviceCaller.CallWithResilienceAsync("api/AcceptedCreditCards");

            if (response != null && response.IsSuccessStatusCode)
            {
                var acceptedPaymentMethods = await response.Content.ReadFromJsonAsync<List<string>>();
                _logger.LogInformation("Erfolgreich {Count} Payment-Methoden erhalten", acceptedPaymentMethods?.Count ?? 0);
                return Ok(acceptedPaymentMethods);
            }

            _logger.LogError("Alle CreditcardService-Instanzen sind nicht erreichbar");
            return StatusCode(503, new
            {
                error = "Service Unavailable",
                message = "Alle CreditcardService-Instanzen sind derzeit nicht erreichbar. Bitte versuchen Sie es spaeter erneut."
            });
        }
    }
}
