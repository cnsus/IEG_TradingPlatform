using System.Xml.Serialization;

namespace PaymentService.Models
{
    [XmlRoot("Payment")]
    public class Payment
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
