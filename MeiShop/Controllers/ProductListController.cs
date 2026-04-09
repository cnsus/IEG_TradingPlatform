using Microsoft.AspNetCore.Mvc;

namespace MeiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductListController : ControllerBase
    {
        private readonly ILogger<ProductListController> _logger;

        // Adresse des ProductService-Microservices
        private static readonly string productServiceUrl = "https://localhost:7200/api/products";

        public ProductListController(ILogger<ProductListController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(productServiceUrl);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Produkte erfolgreich vom ProductService geladen.");
                return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
            }

            _logger.LogError($"ProductService nicht erreichbar. Status: {response.StatusCode}");
            return new List<string> { "Produkte nicht verfügbar" };
        }
    }
}
