using FluentFTP;

namespace FtpProductCatalogService.Services
{
    // Liest eine products.txt vom FTP-Server und gibt die Zeilen als Produktliste zurück.
    // Die Verbindungsdaten kommen aus appsettings.json (Decentralized Data Management).
    public class FtpProductCatalogRepository : IProductCatalogRepository
    {
        private readonly IConfiguration _config;
        private readonly ILogger<FtpProductCatalogRepository> _logger;

        public FtpProductCatalogRepository(IConfiguration config, ILogger<FtpProductCatalogRepository> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetAllAsync()
        {
            var host     = _config["Ftp:Host"]!;
            var port     = _config.GetValue<int>("Ftp:Port", 21);
            var user     = _config["Ftp:Username"]!;
            var pass     = _config["Ftp:Password"]!;
            var filePath = _config["Ftp:FilePath"]!;

            _logger.LogInformation("Verbinde mit FTP-Server {Host}:{Port}...", host, port);

            var config = new FtpConfig
            {
                // Explizites FTPS: Client sendet AUTH TLS vor dem Login
                EncryptionMode = FtpEncryptionMode.Explicit,
                DataConnectionEncryption = true,
                // Selbst-signierte TLS-Zertifikate akzeptieren (für lokale Entwicklung)
                ValidateAnyCertificate = true
            };

            try
            {
                using var ftp = new AsyncFtpClient(host, user, pass, port, config);
                await ftp.Connect();

                using var stream = new MemoryStream();
                await ftp.DownloadStream(stream, filePath);
                stream.Position = 0;

                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();

                _logger.LogInformation("products.txt erfolgreich vom FTP-Server geladen.");

                return ParseProducts(content);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "FTP-Server nicht erreichbar oder Fehler beim Download. Verwende lokalen Fallback-Persistencestore (Lokale products.txt)...");
                
                try
                {
                    // Versuche lokale products.txt zu lesen
                    var localPath = Path.Combine(AppContext.BaseDirectory, "products.txt");
                    if (!File.Exists(localPath))
                    {
                        // Falls die Datei im AppContext nicht existiert, suche im Projektordner
                        localPath = Path.Combine(Directory.GetCurrentDirectory(), "products.txt");
                    }

                    if (File.Exists(localPath))
                    {
                        var content = await File.ReadAllTextAsync(localPath);
                        _logger.LogInformation("Lokale products.txt erfolgreich geladen.");
                        return ParseProducts(content);
                    }
                }
                catch (Exception localEx)
                {
                    _logger.LogError(localEx, "Fehler beim Lesen der lokalen products.txt");
                }

                // Absoluter Fallback, falls auch die Datei fehlt
                return new List<string> { "FTP-Monitor (Hartcodierter Fallback)", "FTP-Keyboard (Hartcodierter Fallback)", "FTP-Mouse (Hartcodierter Fallback)" };
            }
        }

        private IEnumerable<string> ParseProducts(string content)
        {
            return content
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrEmpty(line));
        }
    }
}
