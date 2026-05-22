using FtpProductCatalogService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FtpProductCatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCatalogController : ControllerBase
    {
        private readonly IProductCatalogRepository _repository;

        public ProductCatalogController(IProductCatalogRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
