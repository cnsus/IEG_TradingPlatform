# gRPC LoggingService

Dieser Microservice ist ein zentraler Logging-Server für die IEG Trading Platform, der im Rahmen von **Aufgabe 3** entwickelt wurde. Er nimmt Fehler-Logs von anderen Services (wie dem `MeiShop` API Gateway) über gRPC (HTTP/2) entgegen und persistiert diese.

## 🚀 Funktionalität

- **Zentralisiertes Logging:** Andere Microservices können Fehler an diesen Service senden, anstatt sie nur lokal auszugeben.
- **gRPC API:** Bietet eine performante Schnittstelle auf Basis von Protocol Buffers (`logging.proto`), um strukturierte Log-Nachrichten zu empfangen.
- **Persistenz:** 
  - Konsolen-Output für direkte Sichtbarkeit während der Entwicklung.
  - Speicherung in eine Datei (`logs/error_log.txt`) – diese wird im `bin`-Verzeichnis angelegt (z.B. `bin/Debug/net10.0/logs/error_log.txt`).
- **Resilience-Integration:** Der Service wird in Kombination mit einer Polly Retry-Logik und einem Round Robin Load Balancer aufgerufen. Bei jedem Fehlschlag eines Aufrufs an den `CreditcardService` sendet der `MeiShop` einen Fehlerbericht an diesen LoggingService.

## ⚙️ Technische Details

- **Port:** `http://localhost:5500` (Verwendet HTTP/2 ohne TLS, um Kompatibilitätsprobleme mit gRPC unter macOS leichter zu umgehen).
- **Technologie:** .NET 10, ASP.NET Core gRPC.

---

## 🧪 Wie man die Funktionalität testet

Um die vollständige Resilience-Kette (Round Robin ➔ Retry ➔ gRPC Logging ➔ Failover) zu testen, gehe wie folgt vor:

### 1. Alle Services starten
Starte alle Applikationsteile über das Start-Skript im Hauptverzeichnis der Solution. Dadurch werden der LoggingService, 3 Instanzen des CreditcardService und der MeiShop hochgefahren.

```bash
# MacOS / Linux:
./start-all.sh

# Windows:
start-all.bat
```

### 2. Normale Funktion testen
Wenn alle Services laufen, schicke ein paar Requests an den MeiShop. Durch das *Round Robin Load Balancing* wirst du in den Konsolenfenstern der CreditcardServices sehen, dass die Requests reihum verteilt werden.

```bash
curl -k https://localhost:7024/api/paymentmethods
```

### 3. Fehler provozieren (Eine Instanz stoppen)
Finde die Process IDs (PIDs) der CreditcardServices und stoppe gezielt eine Instanz (z. B. auf Port 7232).

```bash
# Auf MacOS/Linux: Zeige die PIDs der 3 Instanzen
lsof -i :7231 -i :7232 -i :7233 | grep LISTEN

# Stoppe z.B. Instanz 2 auf Port 7232
kill $(lsof -ti :7232)
```
*(Alternativ einfach im Terminal, in dem `start-all.sh` läuft, den Teilprozess beenden).*

### 4. Retry, gRPC Logging und Failover beobachten
Führe erneut einen Request aus. Trifft der *Round Robin Load Balancer* nun die gestoppte Instanz 2, passiert Folgendes im Hintergrund:

1. Der Request schlägt fehl (Connection Refused).
2. Die Polly-Logik im MeiShop versucht den Request **4 Mal erneut** (Retry).
3. **Nach jedem fehlgeschlagenen Retry** sendet der MeiShop über den generierten gRPC-Client verlässliche Fehlerdaten an diesen `LoggingService`.
4. Nach dem 4. Fehler gibt Polly auf ➔ MeiShop führt einen **Failover** durch und geht zur nächsten funktionierenden Instanz (z. B. auf Port 7233).
5. Du als User erhältst trotzdem eine erfolgreiche HTTP 200 API Antwort!

```bash
# Test-Request absetzen
curl -k https://localhost:7024/api/paymentmethods
```

### 5. Logs überprüfen
Schaue dir nun die Ausgabe des LoggingService an.

**In der Konsole (Ausgabe):**
```text
warn: LoggingService.Services.LoggingServiceImpl[0]
      [LOG 38577a30] 2026-04-10T16:01:11.4539160Z | Service: MeiShop | Instanz: https://localhost:7232 | Retry: 4 | Status: Exception | Fehler: Connection refused (localhost:7232) | CorrelationId: d5f4284c
```

**In der Log-Datei:**
Die Datei befindet sich im Build-Ordner des LoggingService, z.B. unter:
`LoggingService/bin/Debug/net10.0/logs/error_log.txt`

Beispielinhalt der Datei `error_log.txt`:
```text
[2026-04-10T16:01:11.4539160Z] ID=38577a30 | Service=MeiShop | Instanz=https://localhost:7232 | Retry=4 | Status=Exception | CorrelationId=d5f4284c | Fehler=Connection refused (localhost:7232)
```
