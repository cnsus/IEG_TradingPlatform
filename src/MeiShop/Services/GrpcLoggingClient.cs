using LoggingService;
using Grpc.Net.Client;

namespace MeiShop.Services
{
    /// <summary>
    /// Wrapper um den generierten gRPC-Client fuer den zentralen LoggingService.
    /// Sendet Fehler-Logs asynchron (fire-and-forget), damit der Hauptfluss nicht blockiert wird.
    /// Falls der gRPC-Logging-Service nicht erreichbar ist, wird auf lokales ILogger zurueckgefallen.
    /// </summary>
    public class GrpcLoggingClient
    {
        private readonly LoggingGrpcService.LoggingGrpcServiceClient _client;
        private readonly ILogger<GrpcLoggingClient> _logger;

        public GrpcLoggingClient(LoggingGrpcService.LoggingGrpcServiceClient client, ILogger<GrpcLoggingClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Sendet einen Fehler-Log-Eintrag an den gRPC LoggingService.
        /// Fire-and-forget: Fehler im Logging duerfen den Hauptfluss nicht stoppen.
        /// </summary>
        public async Task LogErrorAsync(string serviceName, string instanceUrl, string errorMessage,
            string httpStatusCode, int retryAttempt, string correlationId)
        {
            try
            {
                var logEntry = new LogEntry
                {
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    ServiceName = serviceName,
                    InstanceUrl = instanceUrl,
                    ErrorMessage = errorMessage,
                    HttpStatusCode = httpStatusCode,
                    RetryAttempt = retryAttempt,
                    CorrelationId = correlationId
                };

                var response = await _client.LogErrorAsync(logEntry);

                if (response.Success)
                {
                    _logger.LogDebug("Fehler erfolgreich an gRPC LoggingService gesendet (LogId: {LogId})", response.LogId);
                }
                else
                {
                    _logger.LogWarning("gRPC LoggingService hat den Log-Eintrag nicht akzeptiert");
                }
            }
            catch (Exception ex)
            {
                // Fallback: Lokales Logging, wenn gRPC nicht erreichbar ist
                _logger.LogError(ex,
                    "[FALLBACK-LOG] gRPC LoggingService nicht erreichbar. " +
                    "Service={ServiceName}, Instanz={InstanceUrl}, Fehler={ErrorMessage}, " +
                    "Retry={RetryAttempt}, CorrelationId={CorrelationId}",
                    serviceName, instanceUrl, errorMessage, retryAttempt, correlationId);
            }
        }
    }
}
