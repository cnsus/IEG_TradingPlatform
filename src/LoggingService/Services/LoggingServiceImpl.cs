using Grpc.Core;
using System.Collections.Concurrent;

namespace LoggingService.Services
{
    /// <summary>
    /// gRPC Server-Implementierung fuer den zentralen Logging-Service.
    /// Empfaengt Fehler-Logs ueber gRPC, speichert sie in-memory und persistiert sie in einer Datei.
    /// </summary>
    public class LoggingServiceImpl : LoggingGrpcService.LoggingGrpcServiceBase
    {
        private readonly ILogger<LoggingServiceImpl> _logger;
        private static readonly ConcurrentBag<LogEntry> _logs = new();
        private static readonly object _fileLock = new();
        private static readonly string _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        private static readonly string _logFilePath = Path.Combine(_logDirectory, "error_log.txt");

        public LoggingServiceImpl(ILogger<LoggingServiceImpl> logger)
        {
            _logger = logger;

            // Sicherstellen, dass das Logs-Verzeichnis existiert
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
                _logger.LogInformation("Log-Verzeichnis erstellt: {Path}", _logDirectory);
            }
        }

        /// <summary>
        /// Empfaengt einen einzelnen Log-Eintrag, speichert ihn in-memory und schreibt ihn in die Log-Datei.
        /// </summary>
        public override Task<LogResponse> LogError(LogEntry request, ServerCallContext context)
        {
            var logId = Guid.NewGuid().ToString("N")[..8];

            // In-Memory speichern
            _logs.Add(request);

            // Console-Ausgabe (fuer Demo-Zwecke)
            _logger.LogWarning(
                "[LOG {LogId}] {Timestamp} | Service: {ServiceName} | Instanz: {InstanceUrl} | " +
                "Retry: {RetryAttempt} | Status: {StatusCode} | Fehler: {ErrorMessage} | CorrelationId: {CorrelationId}",
                logId,
                request.Timestamp,
                request.ServiceName,
                request.InstanceUrl,
                request.RetryAttempt,
                request.HttpStatusCode,
                request.ErrorMessage,
                request.CorrelationId);

            // In Datei persistieren (thread-safe)
            WriteLogToFile(request, logId);

            return Task.FromResult(new LogResponse
            {
                Success = true,
                LogId = logId
            });
        }

        /// <summary>
        /// Streamt alle gespeicherten Logs (optional gefiltert) an den Client.
        /// </summary>
        public override async Task GetLogs(LogFilter request, IServerStreamWriter<LogEntry> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("GetLogs aufgerufen – Filter: Service={ServiceName}, From={FromTimestamp}",
                request.ServiceName, request.FromTimestamp);

            foreach (var log in _logs)
            {
                // Optionaler Filter nach Service-Name
                if (!string.IsNullOrEmpty(request.ServiceName) &&
                    !log.ServiceName.Equals(request.ServiceName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Optionaler Filter nach Zeitstempel
                if (!string.IsNullOrEmpty(request.FromTimestamp) &&
                    DateTime.TryParse(request.FromTimestamp, out var fromDate) &&
                    DateTime.TryParse(log.Timestamp, out var logDate) &&
                    logDate < fromDate)
                {
                    continue;
                }

                await responseStream.WriteAsync(log);
            }
        }

        /// <summary>
        /// Schreibt einen Log-Eintrag in die Datei (append-only, thread-safe).
        /// </summary>
        private void WriteLogToFile(LogEntry entry, string logId)
        {
            try
            {
                var logLine = $"[{entry.Timestamp}] ID={logId} | Service={entry.ServiceName} | " +
                              $"Instanz={entry.InstanceUrl} | Retry={entry.RetryAttempt} | " +
                              $"Status={entry.HttpStatusCode} | CorrelationId={entry.CorrelationId} | " +
                              $"Fehler={entry.ErrorMessage}";

                lock (_fileLock)
                {
                    File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Schreiben in die Log-Datei: {Path}", _logFilePath);
            }
        }
    }
}
