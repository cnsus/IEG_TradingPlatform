using OrderSagaService.Models;

namespace OrderSagaService.Services
{
    /// <summary>
    /// Lokaler Datastore dieses Microservices (Decentralized Data Management).
    /// Speichert alle Bestellungen in-memory.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private static readonly List<Order> _orders = new();
        private static int _nextId = 1;

        public IEnumerable<Order> GetAll() => _orders;

        public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

        public Order Add(Order order)
        {
            order.Id = _nextId++;
            order.CreatedAt = DateTime.Now;
            _orders.Add(order);
            return order;
        }

        public void Update(Order order)
        {
            // In-Memory: Objekt ist bereits referenziert, kein explizites Update noetig.
        }
    }
}
