using OrderSagaService.Models;

namespace OrderSagaService.Services
{
    /// <summary>
    /// Interface fuer den In-Memory Store der Bestellungen.
    /// </summary>
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll();
        Order? GetById(int id);
        Order Add(Order order);
        void Update(Order order);
    }
}
