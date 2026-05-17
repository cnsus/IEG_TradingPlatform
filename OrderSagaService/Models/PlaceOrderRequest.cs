namespace OrderSagaService.Models
{
    /// <summary>
    /// Eingabeobjekt fuer eine neue Bestellung (vom Client gesendet).
    /// </summary>
    public class PlaceOrderRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string Currency { get; set; } = "EUR";
        public string CustomerName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "CreditCard";
    }
}
