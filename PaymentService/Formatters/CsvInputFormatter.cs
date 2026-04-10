using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using PaymentService.Models;

namespace PaymentService.Formatters
{
    // Custom Input Formatter fuer CSV (HTTP Content Negotiation)
    // Der Client sendet CSV via Content-Type: text/csv Header
    public class CsvInputFormatter : TextInputFormatter
    {
        public CsvInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(Payment);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding effectiveEncoding)
        {
            using var reader = new StreamReader(context.HttpContext.Request.Body, effectiveEncoding);
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
                return await InputFormatterResult.FailureAsync();

            // Header-Zeile ueberspringen falls vorhanden
            if (line.StartsWith("Id,") || line.StartsWith("id,"))
                line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
                return await InputFormatterResult.FailureAsync();

            var parts = line.Split(',');
            if (parts.Length < 5)
                return await InputFormatterResult.FailureAsync();

            var payment = new Payment
            {
                Amount = double.Parse(parts[1], CultureInfo.InvariantCulture),
                Currency = parts[2].Trim(),
                Description = parts[3].Trim(),
                PaymentMethod = parts[4].Trim()
            };

            return await InputFormatterResult.SuccessAsync(payment);
        }
    }
}
