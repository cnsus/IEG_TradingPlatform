# TED 3 - Domain Driven Design (8 Punkte)

## Aufgabenstellung

Nutzen Sie die in Aufgabe 2 entwickelte Makro- und Mikro-Architektur als Ausgangspunkt und konkretisieren Sie den fachlichen und technischen Zuschnitt Ihrer Microservices mit Hilfe von Domain Driven Design. Arbeiten Sie geeignete Bounded Contexts heraus und erläutern Sie die Beziehungen zwischen diesen. Gehen Sie dabei auf geeignete DDD-Konzepte wie Shared Kernel, Customer/Supplier, Conformist, Anticorruption Layer, Separate Ways, Open Host Service und Published Language ein und begründen Sie deren Einsatz oder bewusste Nicht-Verwendung in Ihrem Entwurf. Das Ergebnis dieser Aufgabe soll eine strukturierte Darstellung der Aufgaben der einzelnen Microservices, ihrer Kommunikationswege und -stile, der ausgetauschten Daten einschließlich Formate sowie der Datenhaltung sein (4 Punkte)

Beschreiben Sie kurz (max. eine halbe Seite) zwei alternative Ansätze zu Domain Driven Design (4 Punkte)

## Ausarbeitung

### 1. Bounded Contexts in "most wanTED"

Die "most wanTED" Handelsplattform lässt sich fachlich in vier zentrale Bounded Contexts (BC) unterteilen:

1. **Frontend / Gateway Context (`MeiShop`)**
   - *Aufgabe:* Bündelt Anfragen, stellt das UI bereit und orchestriert Aufrufe an Sub-Systeme.
2. **Catalog Context (`ProductService`, `ProductODataService`, `FtpProductCatalogService`)**
   - *Aufgabe:* Verwaltung von Produkten, Preisen und Katalogdaten.
3. **Payment Context (`PaymentService`, `IEGEasyCreditcardService`, `WebhookSubscriberService`)**
   - *Aufgabe:* Abwicklung von finanziellen Transaktionen und Validierung von Zahlungsmitteln.
4. **Order Context (`OrderSagaService`)**
   - *Aufgabe:* Steuerung des Lebenszyklus einer Bestellung (Warenreservierung, Zahlungsabwicklung, Status-Tracking).

### 2. Beziehungen zwischen den Contexts (Context Mapping)

- **Anticorruption Layer (ACL):** Wird stark im `MeiShop` (Gateway Context) genutzt. Das Gateway übersetzt die domänenspezifischen Antworten des *Catalog Contexts* oder *Payment Contexts* in ein einheitliches Format für den Endnutzer, sodass sich Änderungen in den Backend-Services nicht direkt auf die Frontend-Anwendung auswirken.
- **Open Host Service (OHS) & Published Language:** Der `PaymentService` bietet seine Schnittstelle über eine publizierte, wohldefinierte REST-API an. Durch die Implementierung von Content Negotiation (JSON, XML, CSV) fungiert diese API als *Published Language*, die es beliebigen externen oder internen Clients ermöglicht, den Service über standardisierte Protokolle (HTTP) konsistent zu nutzen.
- **Customer / Supplier:** Zwischen dem `OrderSagaService` (Customer) und dem `PaymentService` (Supplier) besteht eine Customer/Supplier-Beziehung. Der Saga-Orchestrator ist darauf angewiesen, dass der Payment-Service Zahlungen verbucht oder Stornos durchführt, um den Gesamtprozess abzuschließen.
- **Shared Kernel:** Wurde *bewusst nicht verwendet*. Ein Shared Kernel (z. B. eine geteilte DLL für Domänenmodelle) würde eine zu starke Kopplung zwischen den Services erzeugen. Stattdessen nutzt jeder Service seine eigenen, isolierten DTOs (Data Transfer Objects), um absolute Autonomie zu bewahren.
- **Separate Ways:** Der `LoggingService` und die fachlichen Bounded Contexts gehen technisch "Separate Ways" – sie teilen keinerlei fachliche Domänenlogik. Die Anbindung erfolgt rein generisch über gRPC.

### 3. Strukturierte Darstellung der Services

| Microservice | Bounded Context | Kommunikationsstil | Datenformate | Datenhaltung |
|---|---|---|---|---|
| **MeiShop** | Gateway Context | Synchron (REST/HTTP) via Consul | JSON | Keine (Durchreiche) |
| **ProductService** | Catalog Context | Synchron (REST/HTTP) | JSON | In-Memory (List) |
| **PaymentService** | Payment Context | Synchron (REST) & Asynchron (Webhook-HTTP) | JSON, XML, CSV | In-Memory (List) |
| **OrderSagaService** | Order Context | Synchron (REST) zu Sub-Systemen | JSON | In-Memory (Saga State) |
| **IEGEasyCredit...** | Payment Context | Synchron (REST) im Hintergrund | JSON | Keine / In-Memory |
| **LoggingService** | *Cross-Cutting* | Synchron (gRPC) | Protobuf | File (error_log.txt) |

---

### 4. Alternative Architekturansätze zu DDD

**(1) Event-Driven Architecture (EDA) ohne strikte Domänengrenzen:**
Während DDD den Fokus auf fachliche Grenzen (Bounded Contexts) und die Sprache (Ubiquitous Language) legt, rückt eine rein reaktive, ereignisgesteuerte Architektur den asynchronen Datenfluss ins Zentrum. Hier kommunizieren alle Komponenten fast ausschließlich über einen Message-Broker (z. B. Kafka oder RabbitMQ) mittels Events (z. B. `OrderPlaced`, `PaymentProcessed`). Der Fokus liegt auf loser zeitlicher Kopplung und hohem Durchsatz. Oft verschwimmen hierbei jedoch die fachlichen Domänengrenzen, da Daten als Event-Streams kontinuierlich durch das gesamte System fließen und Services eher als pure "Event-Prozessoren" statt als fachliche Hüter ihrer Domäne agieren.

**(2) Datenzentrierte Architektur (Data-Centric Architecture / CRUD):**
Anstatt das Verhalten und die Domänenlogik in den Mittelpunkt zu stellen, fokussiert sich dieser traditionelle Ansatz primär auf das Datenmodell (oft relational in einer zentralen SQL-Datenbank). Services sind primär als dünne Schichten (CRUD-APIs) konzipiert, deren Hauptaufgabe es ist, Tabellen auszulesen und zu beschreiben. Anstelle von komplexen Domänenobjekten gibt es meist anämische Modelle (Anemic Domain Models), bei denen die Logik in "Service-Skripten" oder sogar in Stored Procedures der Datenbank liegt. Dieser Ansatz eignet sich exzellent für datenlastige Anwendungen ohne komplexe Geschäftsregeln, skaliert organisatorisch aber weitaus schlechter als DDD, sobald die Fachlogik komplex wird.
