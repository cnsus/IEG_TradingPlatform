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
            +---------------------+---------------------+
            |                     |                     |
   +--------v--------+  +--------v--------+  +---------v---------+
   | ProductService   |  | FtpProductCatalog|  | PaymentService    |
   | (Local Datastore)|  | (FTP/File-based) |  | (JSON/XML/CSV)    |
   +---------+--------+  +------------------+  +---------+---------+
             |                                           |
             |                                  +--------v--------+
             |                                  | IEGEasyCredit-  |
             |                                  | CardService     |
             |                                  | (Round Robin)   |
             |                                  +-----------------+
             |
   +---------v---------+         +------------------+
   |   Consul           |         |  LoggingService  |
   | (Service Discovery)|         |  (gRPC)          |
   +--------------------+         +------------------+
```

---

## Services

| Service                    | Port  | Beschreibung                                              |
|----------------------------|-------|-----------------------------------------------------------|
| **MeiShop**                | 5148  | Haupt-Webshop — orchestriert Produkt- und Payment-Aufrufe |
| **ProductService**         | 5221  | Produktkatalog mit lokalem Datastore                      |
| **FtpProductCatalogService** | 5171 | Produktkatalog via FTP/File-Persistence                   |
| **IEGEasyCreditCardService** | 5201/5202 | Kreditkarten-Payment (multi-instance, Round Robin)  |
| **PaymentService**         | 5069  | Content-Negotiation-faehiges Payment (JSON/XML/CSV)       |
| **LoggingService**         | 5223  | Zentrales gRPC-basiertes Logging                          |
| **ProductODataService**    | 7500  | OData v4 Produktkatalog mit $filter, $orderby, $select    |
| **Consul**                 | 8500  | Service Discovery & Configuration                         |

---

## Schnellstart

### Voraussetzungen

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
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
cd <ServiceName>
dotnet run
```
