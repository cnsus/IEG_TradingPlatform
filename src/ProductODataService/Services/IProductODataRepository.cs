using ProductODataService.Models;

namespace ProductODataService.Services
{
    /// <summary>
    /// Interface fuer den Produkt-Datenzugriff.
    /// Gibt IQueryable zurueck, damit OData die Abfragen direkt ausfuehren kann.
    /// </summary>
    public interface IProductODataRepository
    {
        IQueryable<Product> GetAll();
        Product? GetById(int id);
    }
}
