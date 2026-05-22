using PaymentService.Models;

namespace PaymentService.Services
{
    public interface IPaymentRepository
    {
        IEnumerable<Payment> GetAll();
        Payment? GetById(int id);
        Payment Add(Payment payment);
    }
}
