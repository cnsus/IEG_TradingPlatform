using System.Text;
using System.Text.Json;
using OrderSagaService.Models;

namespace OrderSagaService.Services
{
    /// <summary>
    /// Saga-Orchestrator (Orchestration-basierter Ansatz).
    /// Koordiniert den Bestellprozess ueber mehrere Microservices hinweg.
    ///
    /// Saga-Schritte:
    ///   1. Order lokal anlegen (Status: Pending)
    ///   2. Produktverfuegbarkeit pruefen (ProductService)
    ///   3. Zahlung erstellen (PaymentService)
    ///   4. Order bestaetigen (Status: Confirmed)
    ///
    /// Bei Fehler in einem Schritt werden Kompensations-Transaktionen ausgefuehrt:
    ///   - Order wird auf Status "Cancelled" gesetzt
    ///   - Der Grund des Fehlers wird dokumentiert
    /// </summary>
    public class OrderSagaOrchestrator : IOrderSagaOrchestrator
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderSagaOrchestrator> _logger;
        private readonly HttpClient _httpClient;

        // URLs der beteiligten Services
        private const string ProductServiceUrl = "https://localhost:7200";
        private const string PaymentServiceUrl = "https://localhost:7400";

        public OrderSagaOrchestrator(IOrderRepository orderRepository, ILogger<OrderSagaOrchestrator> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;

            // Selbst-signierte Zertifikate fuer Entwicklung akzeptieren
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
        }

        public async Task<(Order Order, List<SagaStep> Steps)> PlaceOrderAsync(PlaceOrderRequest request)
        {
            var steps = new List<SagaStep>();

            // ============================================================
            // SCHRITT 1: Order lokal anlegen (Status: Pending)
            // ============================================================
            var order = new Order
            {
                ProductName = request.ProductName,
                Amount = request.Amount,
                Currency = request.Currency,
                CustomerName = request.CustomerName,
                Status = OrderStatus.Pending
            };
            _orderRepository.Add(order);

            steps.Add(new SagaStep
            {
                StepNumber = 1,
                StepName = "Order anlegen",
                Status = "Success",
                Details = $"Order #{order.Id} angelegt (Status: Pending)",
                Timestamp = DateTime.Now
            });

            _logger.LogInformation("Saga gestartet: Order #{OrderId} fuer '{ProductName}'", order.Id, order.ProductName);

            // ============================================================
            // SCHRITT 2: Produktverfuegbarkeit pruefen (ProductService)
            // ============================================================
            try
            {
                var productResponse = await _httpClient.GetAsync($"{ProductServiceUrl}/api/products");

                if (!productResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"ProductService nicht erreichbar (Status: {productResponse.StatusCode})");
                }

                var productsJson = await productResponse.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<string>>(productsJson);

                if (products == null || !products.Any(p => p.Equals(request.ProductName, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception($"Produkt '{request.ProductName}' nicht im Katalog gefunden");
                }

                order.Status = OrderStatus.ProductChecked;

                steps.Add(new SagaStep
                {
                    StepNumber = 2,
                    StepName = "Produktverfuegbarkeit pruefen",
                    Status = "Success",
                    Details = $"Produkt '{request.ProductName}' ist verfuegbar",
                    Timestamp = DateTime.Now
                });

                _logger.LogInformation("Saga Schritt 2: Produkt '{ProductName}' verfuegbar", request.ProductName);
            }
            catch (Exception ex)
            {
                steps.Add(new SagaStep
                {
                    StepNumber = 2,
                    StepName = "Produktverfuegbarkeit pruefen",
                    Status = "Failed",
                    Details = ex.Message,
                    Timestamp = DateTime.Now
                });

                _logger.LogWarning("Saga Schritt 2 fehlgeschlagen: {Error}", ex.Message);

                // KOMPENSATION: Order stornieren
                CompensateOrder(order, steps, $"Produktpruefung fehlgeschlagen: {ex.Message}");
                return (order, steps);
            }

            // ============================================================
            // SCHRITT 3: Zahlung erstellen (PaymentService)
            // ============================================================
            try
            {
                var paymentPayload = new
                {
                    amount = request.Amount,
                    currency = request.Currency,
                    description = $"Bestellung #{order.Id}: {request.ProductName}",
                    paymentMethod = request.PaymentMethod
                };

                var json = JsonSerializer.Serialize(paymentPayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var paymentResponse = await _httpClient.PostAsync($"{PaymentServiceUrl}/api/payments", content);

                if (!paymentResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"PaymentService hat die Zahlung abgelehnt (Status: {paymentResponse.StatusCode})");
                }

                order.Status = OrderStatus.PaymentCreated;

                steps.Add(new SagaStep
                {
                    StepNumber = 3,
                    StepName = "Zahlung erstellen",
                    Status = "Success",
                    Details = $"Zahlung ueber {request.Amount} {request.Currency} erfolgreich erstellt",
                    Timestamp = DateTime.Now
                });

                _logger.LogInformation("Saga Schritt 3: Zahlung fuer Order #{OrderId} erstellt", order.Id);
            }
            catch (Exception ex)
            {
                steps.Add(new SagaStep
                {
                    StepNumber = 3,
                    StepName = "Zahlung erstellen",
                    Status = "Failed",
                    Details = ex.Message,
                    Timestamp = DateTime.Now
                });

                _logger.LogWarning("Saga Schritt 3 fehlgeschlagen: {Error}", ex.Message);

                // KOMPENSATION: Order stornieren
                CompensateOrder(order, steps, $"Zahlungserstellung fehlgeschlagen: {ex.Message}");
                return (order, steps);
            }

            // ============================================================
            // SCHRITT 4: Saga erfolgreich abschliessen
            // ============================================================
            order.Status = OrderStatus.Confirmed;
            order.CompletedAt = DateTime.Now;

            steps.Add(new SagaStep
            {
                StepNumber = 4,
                StepName = "Order bestaetigen",
                Status = "Success",
                Details = $"Order #{order.Id} erfolgreich bestaetigt",
                Timestamp = DateTime.Now
            });

            _logger.LogInformation("Saga erfolgreich abgeschlossen: Order #{OrderId} bestaetigt", order.Id);

            return (order, steps);
        }

        /// <summary>
        /// Kompensations-Transaktion: Setzt die Order auf Status "Cancelled"
        /// und dokumentiert den Grund des Abbruchs.
        /// </summary>
        private void CompensateOrder(Order order, List<SagaStep> steps, string reason)
        {
            order.Status = OrderStatus.Cancelled;
            order.CompletedAt = DateTime.Now;
            order.FailureReason = reason;

            steps.Add(new SagaStep
            {
                StepNumber = steps.Count + 1,
                StepName = "Kompensation: Order stornieren",
                Status = "Compensated",
                Details = $"Order #{order.Id} storniert. Grund: {reason}",
                Timestamp = DateTime.Now
            });

            _logger.LogWarning("Saga-Kompensation: Order #{OrderId} storniert. Grund: {Reason}", order.Id, reason);
        }
    }
}
