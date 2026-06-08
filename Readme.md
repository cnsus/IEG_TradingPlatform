# IEG Trading Platform — "most wanTED"

> Microservice-basierte Handelsplattform | FH Campus 02 — Integrationstechnologien fuer eGovernment

## Team
- Hans Erik Krenn
- Patrick Grüner
- Kevin Ulm

---

## Projektueberblick

Die **IEG Trading Platform** ist eine verteilte Handelsplattform, die auf einer Microservice-Architektur basiert. Das Projekt demonstriert zentrale Konzepte moderner Integrationstechnologien: Domain-Driven Design, Service Discovery, asynchrone Kommunikation, Content Negotiation, Resilience Patterns und mehr.

---

## Architektur

```
                          +------------------+
                          |     MeiShop      |
                          |   (Frontend/API) |
                          +--------+---------+
                                   |
               +-------------------+-------------------+
               |                   |                   |
      +--------v--------+  +-------v--------+  +-------v--------+
      | ProductService   |  | FtpProductCatalog|  | PaymentService |
      | (Local Datastore)|  | (FTP/File-based) |  | (JSON/XML/CSV) |
      +--------+---------+  +--------+---------+  +--------+--------+
               |                    |                    |
               |                    |                    |
   +-----------v-----------+  +-----v------+     +-------v-------+
   | ProductODataService   |  | WebhookSub- |     | IEGEasyCredit |
   | (OData v4 catalog)    |  | scriberSvc  |     | CardService    |
   +-----------------------+  | (webhooks)  |     | (Round Robin)  |
                              +-------------+     +---------------+

                 +----------------+      +-------------------+
                 |   OrderSagaSvc |      |   LoggingService  |
                 |  (Saga Orches.)|      |      (gRPC)       |
                 +----------------+      +-------------------+

                    +-------------------------------+
                    |           Consul               |
                    | (Service Discovery & Config)   |
                    +-------------------------------+

``` 

---

## Services

| Service                    | Port  | Beschreibung                                              |
|----------------------------|-------|-----------------------------------------------------------|
| **MeiShop**                | 7024  | Haupt-Webshop — orchestriert Produkt- und Payment-Aufrufe |
| **ProductService**         | 7200  | Produktkatalog mit lokalem Datastore                      |
| **FtpProductCatalogService** | 7300 | Produktkatalog via FTP/File-Persistence                  |
| **IEGEasyCreditCardService** | 7231-7233 | Kreditkarten-Payment (multi-instance, Round Robin) |
| **PaymentService**         | 7400  | Content-Negotiation-faehiges Payment (JSON/XML/CSV)       |
| **LoggingService**         | 5500  | Zentrales gRPC-basiertes Logging                          |
| **ProductODataService**    | 7500  | OData v4 Produktkatalog mit $filter, $orderby, $select    |
| **WebhookSubscriberService** | 7600 | Webhook-Empfaenger fuer Payment-Benachrichtigungen       |
| **OrderSagaService**       | 7700  | SAGA-Orchestrator fuer verteilte Bestellvorgaenge         |
| **Consul**                 | 8500  | Service Discovery & Configuration                         |

---

## Projektstruktur

```
IEG_TradingPlatform/
├── src/                        # Alle Microservice-Projekte
│   ├── MeiShop/                # API Gateway
│   ├── ProductService/
│   ├── FtpProductCatalogService/
│   ├── IEGEasyCreditcardService/
│   ├── PaymentService/
│   ├── LoggingService/
│   ├── ProductODataService/
│   ├── WebhookSubscriberService/
│   ├── OrderSagaService/
│   └── SimpleWebApiConsoleTest/
├── documentation/              # Gesamte Dokumentation (Aufgaben & TED-Dateien liegen direkt hier)
│   ├── 0Aufgabenstellung.md    # Aufgaben-Checklist
│   ├── Aufgabe1.md .. Aufgabe10.md
│   ├── Bonus_Aufgaben.md
│   ├── TED1_Fachartikelanalyse.md .. TED11_Gesamtloesung.md
│   └── 0ImplementationPlan.md / weitere Dokumente
├── AG-WF-Artefacts/            # Workflow-Artefakte & Projektkontext
├── Consul/                     # Consul Konfiguration
├── SolTradingPlatform.sln      # Solution-Datei
├── start-all.sh / .bat         # Start-Skripte
├── Readme.md                   # Diese Datei
└── .claude/                     # KI-Richtlinien / Tooling (hidden folder)
```

---

## Schnellstart

### Voraussetzungen

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- (Optional) [HashiCorp Consul](https://www.consul.io/) fuer Service Discovery
- (Optional) FTP-Server fuer den FtpProductCatalogService

### Alle Services starten

```bash
# Windows
start-all.bat

# Linux / macOS
./start-all.sh
```

### Einzelnen Service starten

```bash
cd src/<ServiceName>
dotnet run
```
