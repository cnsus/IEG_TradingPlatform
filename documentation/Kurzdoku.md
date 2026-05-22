
# IEG Trading Platform – Kurzdokumentation

## Was ist das Projekt?

Eine **Online-Handelsplattform** (Trading Platform), aufgebaut als **Microservice-Architektur** in .NET. Jeder Service uebernimmt genau eine fachliche Aufgabe und laeuft als eigenstaendige Anwendung.

---

## Architektur auf einen Blick

```
                                        ┌───────────────────────┐
                                        │   gRPC LoggingService │
                                        │   (zentrales Logging) │
                                        └───────────┬───────────┘
                                                    ▲ meldet Fehler
                                                    │
┌──────────┐       REST/JSON       ┌────────────────┴───────────────┐
│  Browser  │◄────────────────────►│          MeiShop               │
│  (Client) │                      │     (API Gateway / Einstieg)   │
└──────────┘                       └──────────┬────────────────────┘
                                              │
                          ┌───────────────────┼───────────────────┐
                          ▼                   ▼                   ▼
                ┌────────────────┐  ┌────────────────┐  ┌────────────────────┐
                │ ProductService │  │ FtpCatalogSvc  │  │ CreditcardService  │
                │ (Produkte)     │  │ (Katalog v.    │  │ (Kreditkarten-     │
                │                │  │  FTP-Server)   │  │  zahlungen)        │
                └────────────────┘  └────────────────┘  │  x3 Instanzen     │
                                                        └────────────────────┘
                ┌────────────────┐
                │ PaymentService │  ◄── Direkt vom Client aufrufbar
                │ (Zahlungen in  │      (JSON, XML oder CSV)
                │  JSON/XML/CSV) │
                └────────────────┘
```

---

## Die Services kurz erklaert

| Service | Aufgabe |
|---|---|
| **MeiShop** | API Gateway – der zentrale Einstiegspunkt, leitet Anfragen an die richtigen Services weiter |
| **ProductService** | Stellt die verfuegbaren Produkte bereit |
| **FtpProductCatalogService** | Laedt einen Produktkatalog von einem externen FTP-Server |
| **IEGEasyCreditcardService** | Validiert und verarbeitet Kreditkartenzahlungen (laeuft in 3 Instanzen fuer Ausfallsicherheit) |
| **PaymentService** | Verwaltet Zahlungen – unterstuetzt JSON, XML und CSV dank Content Negotiation |
| **LoggingService** | Sammelt Fehlermeldungen zentral ueber gRPC |

---

## Was wurde umgesetzt?

### Aufgabe 1–2: Microservice-Grundlagen & DDD
📄 *Dokumentiert in:* `DDD_Microservices_Dokumentation.md`

- Aufbau der Microservice-Architektur nach **Domain-Driven Design (DDD)**
- Jeder Service = ein fachlicher Bereich (Bounded Context)
- Kommunikation ueber **REST/JSON**
- **Content Negotiation** im PaymentService (JSON, XML, CSV)
- **Service Discovery** ueber Consul
- Jeder Service verwaltet seine **eigenen Daten** (dezentrale Datenhaltung)

### Aufgabe 3: Skalierung, Ausfallsicherheit & Logging
📄 *Dokumentiert in:* `Aufgabe3_Dokumentation.md`

- **3 Instanzen** des CreditcardService fuer horizontale Skalierung
- **Round Robin Load Balancing** – Anfragen werden gleichmaessig verteilt
- **Retry-Logik mit Failover** – bei Fehler: bis zu 4 Wiederholungsversuche, dann naechste Instanz (Polly Framework)
- **Zentrales gRPC Logging** – Fehler werden an einen separaten LoggingService gemeldet und in eine Datei geschrieben

### Aufgabe 4: Asynchrone Kommunikation (Theorie)
📄 *Dokumentiert in:* `Aufgabe4_Asynchrone_Kommunikation_Dokumentation.md`

- Theoretische Betrachtung des **Messaging Patterns** als Alternative zu synchronem REST
- Fuenf Kommunikationsstile (Publish/Subscribe, Notifications, etc.)
- Technologievergleich: **Apache Kafka vs. RabbitMQ**
- Bewertung, welche Kommunikation synchron bleiben sollte und was von Messaging profitiert

---

## So startet man das Projekt

**Windows:**
```cmd
start-all.bat
```

**macOS/Linux:**
```bash
./start-all.sh
```

Alle Services starten automatisch auf ihren jeweiligen Ports (7024, 7200, 7231–7233, 7300, 7400, 5500).
