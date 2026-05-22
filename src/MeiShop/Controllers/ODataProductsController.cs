using Microsoft.AspNetCore.Mvc;

namespace MeiShop.Controllers
{
    /// <summary>
    /// OData Client Controller – konsumiert den ProductODataService (Aufgabe 8).
    /// Leitet OData-Query-Parameter ($filter, $orderby, $top, $skip, $select, $count) weiter.
    /// 
    /// Beispiele:
    ///   GET /api/ODataProducts                                        → Alle Produkte
    ///   GET /api/ODataProducts?$filter=Price gt 500                   → Teure Produkte
    ///   GET /api/ODataProducts?$orderby=Price desc&amp;$top=5         → Top 5 teuerste
    ///   GET /api/ODataProducts?$filter=Category eq 'Laptops'         → Nur Laptops
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ODataProductsController : ControllerBase
    {
        private readonly ILogger<ODataProductsController> _logger;

        // Adresse des ProductODataService-Microservices
        private static readonly string odataServiceUrl = "https://localhost:7500/odata/Products";

        public ODataProductsController(ILogger<ODataProductsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Leitet die Anfrage an den OData ProductService weiter.
        /// Alle OData-Query-Parameter werden transparent durchgereicht.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            // OData Query-Parameter aus dem Request uebernehmen und weiterleiten
            var queryString = HttpContext.Request.QueryString.Value ?? "";
            var fullUrl = odataServiceUrl + queryString;

            _logger.LogInformation("OData-Client: Weiterleitung an {Url}", fullUrl);

            using var httpClient = new HttpClient(new HttpClientHandler
            {
                // Selbstsignierte Zertifikate fuer Entwicklung akzeptieren
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

            try
            {
                var response = await httpClient.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("OData-Client: Erfolgreiche Antwort erhalten");
                    return Content(content, "application/json");
                }

                _logger.LogError("OData-Client: Fehler vom Service. Status: {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, new
                {
                    error = "OData Service Error",
                    message = $"Der ProductODataService antwortete mit Status {response.StatusCode}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OData-Client: ProductODataService nicht erreichbar");
                return StatusCode(503, new
                {
                    error = "Service Unavailable",
                    message = "Der ProductODataService ist derzeit nicht erreichbar. Bitte versuchen Sie es spaeter erneut."
                });
            }
        }
    }
}
