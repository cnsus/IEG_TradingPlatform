using PaymentService.Models;

namespace PaymentService.Services
{
    // Lokaler Datastore dieses Microservices (Decentralized Data Management)
    public class PaymentRepository : IPaymentRepository
    {
        private static readonly List<Payment> _payments = new()
        {
            new Payment { Id = 1, Amount = 49.99, Currency = "EUR", Description = "Laptop Zubehoer", PaymentMethod = "CreditCard", CreatedAt = new DateTime(2026, 1, 15) },
            new Payment { Id = 2, Amount = 120.00, Currency = "USD", Description = "Software Lizenz", PaymentMethod = "PayPal", CreatedAt = new DateTime(2026, 2, 20) },
            new Payment { Id = 3, Amount = 9.99, Currency = "EUR", Description = "Monatsabo", PaymentMethod = "Bankeinzug", CreatedAt = new DateTime(2026, 3, 1) }
        };

        private static int _nextId = 4;

        public IEnumerable<Payment> GetAll() => _payments;

        public Payment? GetById(int id) => _payments.FirstOrDefault(p => p.Id == id);

        public Payment Add(Payment payment)
        {
            payment.Id = _nextId++;
            payment.CreatedAt = DateTime.Now;
            _payments.Add(payment);
            return payment;
        }
    }
}
