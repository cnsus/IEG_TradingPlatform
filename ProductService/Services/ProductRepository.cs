namespace ProductService.Services
{
    // Lokaler Datastore dieses Microservices (Decentralized Data Management)
    // Jeder Microservice verwaltet seine eigenen Daten – kein geteiltes Datenbankschema.
    public class ProductRepository : IProductRepository
    {
        private static readonly List<string> _products = new()
        {
            "Laptop",
            "Smartphone",
            "Tablet",
            "Headphones",
            "Smartwatch"
        };

        public IEnumerable<string> GetAll() => _products;
    }
}
