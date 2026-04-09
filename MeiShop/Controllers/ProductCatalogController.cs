using Microsoft.AspNetCore.Mvc;

namespace MeiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCatalogController : ControllerBase
    {
        private readonly ILogger<ProductCatalogController> _logger;

        // Adresse des FtpProductCatalogService-Microservices
        private static readonly string ftpCatalogServiceUrl = "https://localhost:7300/api/productcatalog";

        public ProductCatalogController(ILogger<ProductCatalogController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(ftpCatalogServiceUrl);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Produktkatalog erfolgreich vom FtpProductCatalogService geladen.");
                return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
            }

            _logger.LogError($"FtpProductCatalogService nicht erreichbar. Status: {response.StatusCode}");
            return new List<string> { "Katalog nicht verfügbar" };
        }
    }
}
