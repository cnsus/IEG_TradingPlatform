using OrderSagaService.Models;

namespace OrderSagaService.Services
{
    /// <summary>
    /// Interface fuer den Saga-Orchestrator.
    /// </summary>
    public interface IOrderSagaOrchestrator
    {
        Task<(Order Order, List<SagaStep> Steps)> PlaceOrderAsync(PlaceOrderRequest request);
    }
}
