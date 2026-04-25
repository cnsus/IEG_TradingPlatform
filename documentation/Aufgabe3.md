# Aufgabe 3 - Skalierung, Ausfallsicherheit und Logging (10 Punkte)

## Aufgabenstellung

Skalierung, Ausfallssicherheit und Logging (Design for failure) für CreditPaymentService. Detailsbeschreibung: Publizieren Sie das Service „IEGEasyCreditCardService" mehrfach und rufen Sie die Services im „Round Robin" Stil auf. Falls es beim Aufruf eines Service zu einem Fehler kommt, soll es eine Retry-Logik geben, außerdem soll der aufgetretene Fehler mit Hilfe eines zentralen Logging-Service (gRPC) protokolliert werden. Nach n erfolglosen Versuchen, soll das nächste Service aufgerufen werden. Recherchieren Sie zusätzlich nach einem geeigneten Framework und Skalierungsmöglichkeiten setzen Sie dieses gegebenenfalls ein (10 Punkte)

## Ausarbeitung

### 1. Ueberblick

### Implementierte Features

| Feature | Beschreibung |
|---|---|
| **Multi-Instanzen** | 3 Instanzen des IEGEasyCreditcardService auf Ports 7231, 7232, 7233 |
| **Round Robin Load Balancing** | Thread-safe Verteilung der Requests auf alle Instanzen |
| **Retry-Logik** | 4 Retries pro Instanz mit exponentiellem Backoff (Polly) |
| **Failover** | Automatischer Wechsel zur naechsten Instanz nach 4 fehlgeschlagenen Versuchen |
| **Zentrales gRPC Logging** | Fehler werden ueber gRPC an einen separaten LoggingService gesendet und in eine Datei persistiert |

---

### 2. Architektur

```
                            ┌────────────┐
                            │   Client   │
                            │  (Browser) │
                            └─────┬──────┘
                                  │ REST / JSON
                ┌─────────────────┼─────────────────┐
                │                 │                 │
                ▼                 ▼                 ▼
       ┌────────────────┐  ┌────────────┐   (weitere direkte
       │    MeiShop     │  │  Payment   │    Service-Aufrufe)
       │  API Gateway   │  │  Service   │
       │     :7024      │  │   :7400    │
       │                │  │ JSON/XML   │
       │ ┌────────────┐ │  │   / CSV    │
       │ │RoundRobin  │ │  └────────────┘
       │ │LoadBalancer│ │
       │ │+ Resilient │ │
       │ │ServiceCall │ │
       │ │(Polly 4×)  │ │
       │ └─────┬──────┘ │
       └───┬───┼────────┘
           │   │                    ┌─────────────────────────┐
           │   │         gRPC       │    gRPC LoggingService  │
           │   │  (bei Fehlern)────►│         :5500           │
           │   │                    │  [logs/error_log.txt]   │
           │   │                    └─────────────────────────┘
           │   │
           │   │ Round Robin + Failover
           │   ├──────────────────┬──────────────────┐
           │   ▼                  ▼                  ▼
           │ ┌────────────┐  ┌────────────┐  ┌────────────┐
           │ │Creditcard  │  │Creditcard  │  │Creditcard  │
           │ │Svc Inst 1  │  │Svc Inst 2  │  │Svc Inst 3  │
           │ │  :7231     │  │  :7232     │  │  :7233     │
           │ └────────────┘  └────────────┘  └────────────┘
           │
           ├──────────────────┐
           ▼                  ▼
     ┌────────────┐   ┌──────────────┐
     │  Product   │   │ FtpProduct   │
     │  Service   │   │CatalogService│
     │   :7200    │   │    :7300     │
     └────────────┘   └──────────────┘
```

**Legende:**
- **MeiShop** routet Shop-Anfragen an `ProductService`, `FtpProductCatalogService` und die drei `CreditcardService`-Instanzen
- Nur die Aufrufe an den `CreditcardService` nutzen **RoundRobin + Retry + Failover** (Aufgabe 3)
- **PaymentService** wird direkt vom Client angesprochen (Content Negotiation, eigener Bounded Context)
- Der **gRPC LoggingService** wird von MeiShop bei Fehlern im Resilient-Caller angesprochen

---

### 3. Komponenten im Detail

#### 3.1 Round Robin Load Balancer

**Datei:** `MeiShop/Services/RoundRobinLoadBalancer.cs`

Der `RoundRobinLoadBalancer` verteilt Anfragen gleichmaessig auf alle konfigurierten Service-Instanzen:

- **Thread-Safe:** Verwendet `Interlocked.Increment` fuer atomaren Index-Zugriff bei gleichzeitigen Requests
- **Singleton:** Wird als Singleton registriert, damit der Rotations-Zustand ueber alle Requests hinweg erhalten bleibt
- **Konfigurierbar:** Instanz-URLs werden aus `appsettings.json` geladen

**Funktionsweise:**
```
Request 1 → Instanz 1 (Port 7231)
Request 2 → Instanz 2 (Port 7232)
Request 3 → Instanz 3 (Port 7233)
Request 4 → Instanz 1 (Port 7231)  ← Round Robin beginnt von vorne
...
```

#### 3.2 Resilient Service Caller (Retry + Failover)

**Datei:** `MeiShop/Services/ResilientServiceCaller.cs`

Der `ResilientServiceCaller` implementiert das **"Design for failure"** Prinzip:

**Ablauf bei einem Request:**
1. **Instanz waehlen** via Round Robin Load Balancer
2. **Request ausfuehren** an die gewaehlte Instanz
3. **Bei Fehler:** Polly Retry-Policy greift (bis zu 4 Versuche)
   - Exponentielles Backoff: 2s → 4s → 6s → 8s
   - Jeder fehlgeschlagene Versuch wird an den gRPC LoggingService gesendet
4. **Nach 4 fehlgeschlagenen Retries:** Failover zur naechsten Instanz
5. **Alle 3 Instanzen fehlgeschlagen:** HTTP 503 (Service Unavailable) zurueckgeben

#### 3.3 Polly Resilience Framework

**Polly** (https://github.com/App-vNext/Polly) ist der De-facto-Standard fuer Resilience und transiente Fehlerbehandlung in .NET. Es bietet:

- **Retry:** Automatisches Wiederholen fehlgeschlagener Operationen
- **Circuit Breaker:** Schutz vor kaskadierten Fehlern
- **Timeout:** Begrenzung der Wartezeit
- **Fallback:** Alternative Aktionen bei Fehlern

In dieser Implementierung nutzen wir `WaitAndRetryAsync` mit exponentiellem Backoff:

```csharp
Policy.HandleResult<HttpResponseMessage>(msg => !msg.IsSuccessStatusCode)
    .Or<HttpRequestException>()
    .Or<TaskCanceledException>()
    .WaitAndRetryAsync(
        4,  // MaxRetriesPerInstance
        retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt),  // Exponential Backoff
        (outcome, timeSpan, retryCount, context) => { /* Logging */ });
```

#### 3.4 gRPC Logging-Service

**Verzeichnis:** `LoggingService/`

Ein eigenstaendiger Microservice, der Fehler-Logs zentral entgegennimmt und persistiert.

**Technologie:** gRPC mit Protocol Buffers (protobuf)

**Warum gRPC?**
- **Performance:** Binaeres Protokoll (protobuf) ist effizienter als JSON/XML
- **Typsicherheit:** Streng typisierte Nachrichten durch `.proto`-Definitionen
- **Streaming:** Server-Streaming fuer Log-Abruf (GetLogs RPC)
- **Aufgabenstellung:** Explizit gRPC gefordert

**Proto-Definition (logging.proto):**

```protobuf
service LoggingGrpcService {
  rpc LogError (LogEntry) returns (LogResponse);       // Fehler loggen
  rpc GetLogs (LogFilter) returns (stream LogEntry);   // Logs abrufen
}

message LogEntry {
  string timestamp = 1;          // Zeitpunkt (ISO 8601)
  string service_name = 2;       // Aufrufender Service
  string instance_url = 3;       // Fehlgeschlagene Instanz-URL
  string error_message = 4;      // Fehlerbeschreibung
  string http_status_code = 5;   // HTTP Status Code
  int32 retry_attempt = 6;       // Retry-Versuch
  string correlation_id = 7;     // Request-Korrelation
}
```

**Persistenz:**
- **In-Memory:** `ConcurrentBag<LogEntry>` fuer schnellen Zugriff
- **Datei:** `logs/error_log.txt` (append-only, thread-safe via Lock)
- **Console:** Ausgabe fuer Demo-Zwecke

#### 3.5 gRPC Client (MeiShop)

**Datei:** `MeiShop/Services/GrpcLoggingClient.cs`

- **Fire-and-forget:** Logging blockiert den Hauptfluss nicht
- **Fallback:** Bei unerreichbarem LoggingService wird auf lokales `ILogger` zurueckgefallen
- **Singleton:** Ein gRPC-Channel wird wiederverwendet

---

### 4. Bezug zu Microservices-Konzepten

#### 4.1 Design for failure (Martin Fowler)

Laut dem Artikel von Martin Fowler zu Microservices muessen Services so entworfen werden, dass sie mit dem Ausfall anderer Services umgehen koennen:

> *"A consequence of using services as components, is that applications need to be designed so that they can tolerate the failure of services."*

Unsere Implementierung setzt dies um durch:
- **Retry-Logik:** Automatische Wiederholungsversuche bei transienten Fehlern
- **Failover:** Wechsel zur naechsten Instanz bei dauerhaften Fehlern
- **Graceful Degradation:** HTTP 503 mit informativer Fehlermeldung statt Absturz

#### 4.2 Skalierung

Die Multi-Instanz-Konfiguration demonstriert **horizontale Skalierung**:
- Gleicher Service-Code laeuft auf mehreren Ports
- Load Balancing verteilt die Last gleichmaessig
- Instanzen koennen unabhaengig gestartet und gestoppt werden

#### 4.3 Zentrales Logging

In einer Microservice-Architektur ist **zentrales Logging** essentiell:
- **Verteilte Systeme:** Fehler koennen in jedem Service auftreten
- **Korrelation:** Durch `correlation_id` koennen Requests ueber Service-Grenzen nachverfolgt werden
- **Monitoring:** Der LoggingService bietet eine zentrale Stelle fuer Fehleranalyse

---

### 5. Konfiguration

#### MeiShop appsettings.json

```json
{
  "CreditcardService": {
    "Instances": [
      "https://localhost:7231",
      "https://localhost:7232",
      "https://localhost:7233"
    ],
    "MaxRetriesPerInstance": 4,
    "RetryWaitSeconds": 2
  },
  "LoggingService": {
    "Address": "http://localhost:5500"
  }
}
```

| Parameter | Wert | Beschreibung |
|---|---|---|
| `Instances` | 3 URLs | CreditcardService-Instanzen fuer Round Robin |
| `MaxRetriesPerInstance` | 4 | Anzahl Versuche pro Instanz vor Failover |
| `RetryWaitSeconds` | 2 | Basis-Wartezeit (wird mit Versuchsnummer multipliziert) |
| `LoggingService:Address` | http://localhost:5500 | gRPC LoggingService Adresse |

---

### 6. Starten und Testen

#### Services starten

**macOS/Linux:**
```bash
./start-all.sh
```

**Windows:**
```cmd
start-all.bat
```

#### Testen

**1. Round Robin testen (10 Requests):**
```bash
for i in {1..10}; do
  echo "Request $i:"
  curl -k https://localhost:7024/api/paymentmethods
  echo ""
done
```

**2. Failover testen:**
- Instanz 2 stoppen (Terminal schliessen oder Ctrl+C)
- Weitere Requests senden → 4 Retries, dann Failover beobachten

**3. Logs pruefen:**
- Console des LoggingService beobachten
- Datei `LoggingService/logs/error_log.txt` pruefen

#### Port-Uebersicht

| Service | Port | Protokoll |
|---|---|---|
| LoggingService (gRPC) | 5500 | HTTP/2 (gRPC) |
| CreditcardService #1 | 7231 | HTTPS (REST) |
| CreditcardService #2 | 7232 | HTTPS (REST) |
| CreditcardService #3 | 7233 | HTTPS (REST) |
| ProductService | 7200 | HTTPS (REST) |
| FtpProductCatalogService | 7300 | HTTPS (REST) |
| PaymentService | 7400 | HTTPS (REST) |
| MeiShop (API Gateway) | 7024 | HTTPS (REST) |

---

### 7. Neue und geaenderte Dateien

#### Neue Dateien

| Datei | Beschreibung |
|---|---|
| `LoggingService/LoggingService.csproj` | Neues gRPC-Server-Projekt |
| `LoggingService/Program.cs` | gRPC-Server Startup |
| `LoggingService/Protos/logging.proto` | gRPC Service-Definition |
| `LoggingService/Services/LoggingServiceImpl.cs` | gRPC-Implementierung |
| `LoggingService/appsettings.json` | Konfiguration |
| `MeiShop/Services/RoundRobinLoadBalancer.cs` | Round-Robin Load Balancer |
| `MeiShop/Services/ResilientServiceCaller.cs` | Retry + Failover Logik |
| `MeiShop/Services/GrpcLoggingClient.cs` | gRPC-Client fuer Logging |
| `MeiShop/Protos/logging.proto` | gRPC Client-Definition |
| `start-all.sh` | macOS Start-Skript |

#### Geaenderte Dateien

| Datei | Aenderung |
|---|---|
| `MeiShop/MeiShop.csproj` | gRPC NuGet-Pakete + Proto-Referenz |
| `MeiShop/appsettings.json` | Instanz-URLs + LoggingService-Adresse |
| `MeiShop/Program.cs` | DI-Registrierung aller neuen Services |
| `MeiShop/Controllers/PaymentMethodsController.cs` | ResilientServiceCaller statt hardcoded URL |
| `SolTradingPlatform.sln` | LoggingService-Projekt hinzugefuegt |
| `start-all.bat` | 3 CreditcardService-Instanzen + LoggingService |
