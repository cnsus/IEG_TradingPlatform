using LoggingService.Services;

namespace LoggingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // gRPC-Services registrieren
            builder.Services.AddGrpc();

            // Kestrel fuer HTTP/2 (gRPC) konfigurieren
            // HTTP statt HTTPS fuer macOS-Kompatibilitaet
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(5500, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
            });

            var app = builder.Build();

            // gRPC-Service Endpunkt registrieren
            app.MapGrpcService<LoggingServiceImpl>();

            // Info-Endpunkt fuer Browser-Zugriffe
            app.MapGet("/", () => "gRPC LoggingService laeuft. Kommunikation ueber gRPC-Client (Port 5500).");

            Console.WriteLine("========================================");
            Console.WriteLine("  gRPC LoggingService gestartet");
            Console.WriteLine("  Port: http://localhost:5500");
            Console.WriteLine("  Logs: logs/error_log.txt");
            Console.WriteLine("========================================");

            app.Run();
        }
    }
}
