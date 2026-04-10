using Polly;
using System.Net.Http.Headers;

namespace MeiShop.Services
{
    /// <summary>
    /// Resilient Service Caller mit Round-Robin Load Balancing, Polly Retry und Failover.
    /// 
    /// Ablauf:
    /// 1. Waehle Instanz via Round Robin
    /// 2. Versuche Aufruf (bis zu MaxRetriesPerInstance Retries mit Polly)
    /// 3. Bei Fehler in jedem Retry → Sende Fehlerdetails an gRPC LoggingService
    /// 4. Nach MaxRetriesPerInstance erfolglosen Retries → Waehle naechste Instanz
    /// 5. Nach Durchlauf aller Instanzen → Endgueltige Fehlermeldung
    /// </summary>
    public class ResilientServiceCaller
    {
        private readonly RoundRobinLoadBalancer _loadBalancer;
        private readonly GrpcLoggingClient _grpcLogger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ResilientServiceCaller> _logger;
        private readonly int _maxRetriesPerInstance;
        private readonly int _retryWaitSeconds;

        public ResilientServiceCaller(
            RoundRobinLoadBalancer loadBalancer,
            GrpcLoggingClient grpcLogger,
            IHttpClientFactory httpClientFactory,
            ILogger<ResilientServiceCaller> logger,
            IConfiguration configuration)
        {
            _loadBalancer = loadBalancer;
            _grpcLogger = grpcLogger;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _maxRetriesPerInstance = configuration.GetValue<int>("CreditcardService:MaxRetriesPerInstance", 4);
            _retryWaitSeconds = configuration.GetValue<int>("CreditcardService:RetryWaitSeconds", 2);
        }

        /// <summary>
        /// Fuehrt einen GET-Request mit Round-Robin, Retry und Failover durch.
        /// </summary>
        /// <param name="apiPath">Relativer API-Pfad (z.B. "api/AcceptedCreditCards")</param>
        /// <returns>HttpResponseMessage bei Erfolg, null wenn alle Instanzen fehlgeschlagen</returns>
        public async Task<HttpResponseMessage?> CallWithResilienceAsync(string apiPath)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            var totalInstances = _loadBalancer.InstanceCount;
            var currentInstance = _loadBalancer.GetNextInstance();

            _logger.LogInformation("[{CorrelationId}] Starte resilienten Aufruf fuer {ApiPath}. " +
                                   "Erste Instanz: {Instance}", correlationId, apiPath, currentInstance);

            // Ueber alle verfuegbaren Instanzen iterieren (Failover)
            for (int instanceAttempt = 0; instanceAttempt < totalInstances; instanceAttempt++)
            {
                _logger.LogInformation("[{CorrelationId}] Versuche Instanz {Instance} (Instanz-Versuch {Attempt}/{Total})",
                    correlationId, currentInstance, instanceAttempt + 1, totalInstances);

                var result = await TryInstanceWithRetryAsync(currentInstance, apiPath, correlationId);

                if (result != null && result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("[{CorrelationId}] Erfolg auf Instanz {Instance}", 
                        correlationId, currentInstance);
                    return result;
                }

                // Alle Retries auf dieser Instanz fehlgeschlagen → naechste Instanz
                _logger.LogWarning("[{CorrelationId}] Alle {MaxRetries} Retries auf Instanz {Instance} fehlgeschlagen. " +
                                   "Failover zur naechsten Instanz...",
                    correlationId, _maxRetriesPerInstance, currentInstance);

                currentInstance = _loadBalancer.GetNextInstanceAfter(currentInstance);
            }

            // Alle Instanzen durchprobiert ohne Erfolg
            _logger.LogError("[{CorrelationId}] ALLE {TotalInstances} Instanzen fehlgeschlagen fuer {ApiPath}. " +
                             "Keine weiteren Instanzen verfuegbar.",
                correlationId, totalInstances, apiPath);

            return null;
        }

        /// <summary>
        /// Versucht den Aufruf einer einzelnen Instanz mit Polly Retry-Policy.
        /// </summary>
        private async Task<HttpResponseMessage?> TryInstanceWithRetryAsync(string instanceUrl, string apiPath, string correlationId)
        {
            var retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(msg => !msg.IsSuccessStatusCode)
                .Or<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    _maxRetriesPerInstance,
                    retryAttempt => TimeSpan.FromSeconds(_retryWaitSeconds * retryAttempt), // Exponential Backoff
                    async (outcome, timeSpan, retryCount, context) =>
                    {
                        var statusCode = outcome.Result?.StatusCode.ToString() ?? "ConnectionError";
                        var errorMessage = outcome.Exception?.Message ?? 
                                           $"HTTP {outcome.Result?.StatusCode}";

                        _logger.LogWarning(
                            "[{CorrelationId}] Retry {RetryCount}/{MaxRetries} auf Instanz {Instance} fehlgeschlagen. " +
                            "Status: {StatusCode}. Naechster Versuch in {Delay}s",
                            correlationId, retryCount, _maxRetriesPerInstance, instanceUrl, statusCode, timeSpan.TotalSeconds);

                        // Fehler an gRPC LoggingService senden (fire-and-forget)
                        await _grpcLogger.LogErrorAsync(
                            serviceName: "MeiShop",
                            instanceUrl: instanceUrl,
                            errorMessage: errorMessage,
                            httpStatusCode: statusCode,
                            retryAttempt: retryCount,
                            correlationId: correlationId);
                    });

            try
            {
                var client = _httpClientFactory.CreateClient("CreditcardService");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var fullUrl = $"{instanceUrl.TrimEnd('/')}/{apiPath.TrimStart('/')}";

                return await retryPolicy.ExecuteAsync(() => client.GetAsync(fullUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{CorrelationId}] Unerwarteter Fehler beim Aufruf von {Instance}/{ApiPath}",
                    correlationId, instanceUrl, apiPath);

                // Auch unerwartete Fehler loggen
                await _grpcLogger.LogErrorAsync(
                    serviceName: "MeiShop",
                    instanceUrl: instanceUrl,
                    errorMessage: ex.Message,
                    httpStatusCode: "Exception",
                    retryAttempt: _maxRetriesPerInstance,
                    correlationId: correlationId);

                return null;
            }
        }
    }
}
