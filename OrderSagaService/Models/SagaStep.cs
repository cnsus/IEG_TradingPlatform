namespace OrderSagaService.Models
{
    /// <summary>
    /// Repraesentiert einen einzelnen Saga-Schritt im Ausfuehrungsprotokoll.
    /// Dient der Nachvollziehbarkeit und dem Tracing der Saga.
    /// </summary>
    public class SagaStep
    {
        public int StepNumber { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Success, Failed, Compensated
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
