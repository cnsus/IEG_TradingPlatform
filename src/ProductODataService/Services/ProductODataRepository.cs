using ProductODataService.Models;

namespace ProductODataService.Services
{
    /// <summary>
    /// Lokaler Datastore dieses Microservices (Decentralized Data Management).
    /// Stellt Produktdaten als IQueryable fuer OData-Abfragen bereit.
    /// Jeder Microservice verwaltet seine eigenen Daten – kein geteiltes Datenbankschema.
    /// </summary>
    public class ProductODataRepository : IProductODataRepository
    {
        private static readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Gaming Laptop", Description = "High-End Gaming Laptop mit RTX 4090", Price = 2499.99m, Category = "Laptops", StockQuantity = 15, Vendor = "TechStore", CreatedAt = new DateTime(2026, 1, 10), IsAvailable = true },
            new Product { Id = 2, Name = "Business Ultrabook", Description = "Leichtes Ultrabook fuer Geschaeftsreisen", Price = 1299.99m, Category = "Laptops", StockQuantity = 30, Vendor = "TechStore", CreatedAt = new DateTime(2026, 1, 15), IsAvailable = true },
            new Product { Id = 3, Name = "Smartphone Pro", Description = "Flagship Smartphone mit 200MP Kamera", Price = 999.99m, Category = "Smartphones", StockQuantity = 50, Vendor = "MobileWorld", CreatedAt = new DateTime(2026, 2, 1), IsAvailable = true },
            new Product { Id = 4, Name = "Budget Phone", Description = "Guenstiges Smartphone fuer den Alltag", Price = 199.99m, Category = "Smartphones", StockQuantity = 100, Vendor = "MobileWorld", CreatedAt = new DateTime(2026, 2, 5), IsAvailable = true },
            new Product { Id = 5, Name = "Wireless Headphones", Description = "Noise-Cancelling Over-Ear Kopfhoerer", Price = 349.99m, Category = "Audio", StockQuantity = 45, Vendor = "AudioMax", CreatedAt = new DateTime(2026, 2, 10), IsAvailable = true },
            new Product { Id = 6, Name = "Smartwatch Elite", Description = "Premium Smartwatch mit Gesundheitstracking", Price = 449.99m, Category = "Wearables", StockQuantity = 25, Vendor = "WearTech", CreatedAt = new DateTime(2026, 3, 1), IsAvailable = true },
            new Product { Id = 7, Name = "Tablet 12 Zoll", Description = "Grosses Tablet fuer Kreative", Price = 799.99m, Category = "Tablets", StockQuantity = 20, Vendor = "TechStore", CreatedAt = new DateTime(2026, 3, 10), IsAvailable = true },
            new Product { Id = 8, Name = "USB-C Hub", Description = "7-in-1 USB-C Docking Station", Price = 59.99m, Category = "Zubehoer", StockQuantity = 200, Vendor = "TechStore", CreatedAt = new DateTime(2026, 3, 15), IsAvailable = true },
            new Product { Id = 9, Name = "Mechanische Tastatur", Description = "RGB Gaming Tastatur mit Cherry MX Switches", Price = 149.99m, Category = "Zubehoer", StockQuantity = 60, Vendor = "AudioMax", CreatedAt = new DateTime(2026, 3, 20), IsAvailable = true },
            new Product { Id = 10, Name = "4K Monitor", Description = "32 Zoll 4K IPS Monitor fuer Profis", Price = 599.99m, Category = "Monitore", StockQuantity = 0, Vendor = "TechStore", CreatedAt = new DateTime(2026, 4, 1), IsAvailable = false }
        };

        public IQueryable<Product> GetAll() => _products.AsQueryable();

        public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
    }
}
