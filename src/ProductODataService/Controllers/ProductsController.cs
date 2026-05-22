using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ProductODataService.Models;
using ProductODataService.Services;

namespace ProductODataService.Controllers
{
    /// <summary>
    /// OData-faehiger Controller fuer Produktabfragen.
    /// Unterstuetzt $filter, $orderby, $top, $skip, $count, $select.
    /// 
    /// Beispielabfragen:
    ///   GET /odata/Products                                          → Alle Produkte
    ///   GET /odata/Products?$filter=Price gt 500                     → Produkte teurer als 500€
    ///   GET /odata/Products?$orderby=Price desc&amp;$top=5           → Top 5 teuerste Produkte
    ///   GET /odata/Products?$filter=Category eq 'Laptops'&amp;$select=Name,Price → Nur Laptop-Namen und Preise
    ///   GET /odata/Products?$count=true&amp;$top=3                   → Erste 3 Produkte mit Gesamtanzahl
    ///   GET /odata/Products(1)                                       → Einzelnes Produkt mit Id 1
    /// </summary>
    public class ProductsController : ODataController
    {
        private readonly IProductODataRepository _repository;

        public ProductsController(IProductODataRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gibt alle Produkte zurueck. OData-Query-Parameter werden automatisch angewendet.
        /// </summary>
        [EnableQuery(PageSize = 20)]
        public IActionResult Get()
        {
            return Ok(_repository.GetAll());
        }

        /// <summary>
        /// Gibt ein einzelnes Produkt anhand der ID zurueck.
        /// </summary>
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var product = _repository.GetById(key);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}
