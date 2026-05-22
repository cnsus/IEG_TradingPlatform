using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using PaymentService.Models;

namespace PaymentService.Formatters
{
    // Custom Output Formatter fuer CSV (HTTP Content Negotiation)
    // Der Client fordert CSV an via Accept: text/csv Header
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type? type)
        {
            if (type == typeof(Payment) || type == typeof(List<Payment>) || type == typeof(IEnumerable<Payment>))
                return true;
            return false;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var sb = new StringBuilder();

            // CSV Header
            sb.AppendLine("Id,Amount,Currency,Description,PaymentMethod,CreatedAt");

            if (context.Object is IEnumerable<Payment> payments)
            {
                foreach (var payment in payments)
                {
                    sb.AppendLine(FormatPayment(payment));
                }
            }
            else if (context.Object is Payment single)
            {
                sb.AppendLine(FormatPayment(single));
            }

            await response.WriteAsync(sb.ToString(), selectedEncoding);
        }

        private static string FormatPayment(Payment p)
        {
            return $"{p.Id},{p.Amount.ToString(CultureInfo.InvariantCulture)},{p.Currency},{p.Description},{p.PaymentMethod},{p.CreatedAt:yyyy-MM-dd}";
        }
    }
}
