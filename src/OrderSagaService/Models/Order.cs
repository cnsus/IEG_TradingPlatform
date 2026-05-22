namespace OrderSagaService.Models
{
    /// <summary>
    /// Repraesentiert eine Bestellung innerhalb der Saga.
    /// Der Status spiegelt den aktuellen Zustand im Saga-Workflow wider.
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string Currency { get; set; } = "EUR";
        public string CustomerName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
    }

    /// <summary>
    /// Zustandsautomat der Saga: Pending → Confirmed / Cancelled
    /// </summary>
    public enum OrderStatus
    {
        Pending,        // Saga gestartet, noch nicht abgeschlossen
        ProductChecked, // Produktverfuegbarkeit geprueft
        PaymentCreated, // Zahlung erfolgreich erstellt
        Confirmed,      // Saga erfolgreich abgeschlossen
        Cancelled       // Saga fehlgeschlagen, Kompensation durchgefuehrt
    }
}
