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
            "Smartwatch",
            "NVIDIA RTX 5090",
            "Intel Core i9-15900K",
            "AMD Ryzen 9 9950X",
            "Corsair Vengeance 128GB DDR5",
            "Samsung Odyssey Neo G9",
            "Logitech G Pro X Superlight 2"
        };

        public IEnumerable<string> GetAll() => _products;
    }
}
