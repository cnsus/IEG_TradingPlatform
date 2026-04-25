namespace ProductODataService.Models
{
    /// <summary>
    /// Produkt-Entitaet fuer den OData-Service.
    /// Ermoeglicht $filter, $orderby, $select, $top, $skip Abfragen
    /// ueber das OData v4 Protokoll.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public string Vendor { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAvailable { get; set; }
    }
}
