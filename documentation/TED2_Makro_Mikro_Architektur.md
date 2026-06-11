# TED 2 - Makro- und Mikro-Architektur (8 Punkte)

## Aufgabenstellung

Beschreiben Sie die Makro- und Mikro-Architektur Ihrer Lösung zum Thema „most wanTED". Stellen Sie auf Makro-Ebene die zentralen fachlichen Bausteine, die daraus abgeleiteten Microservices sowie deren Kommunikationsbeziehungen dar. Erläutern Sie auf Mikro-Ebene beispielhaft den inneren Aufbau von ein bis zwei Microservices, insbesondere im Hinblick auf Schnittstellen, Geschäftslogik und Datenhaltung. Begründen Sie kurz, inwiefern Ihre Architektur die Anforderungen an Erweiterbarkeit, Austauschbarkeit und Skalierbarkeit unterstützt.

## Ausarbeitung

### 1. Makro-Architektur der "most wanTED" Plattform

Auf der Makro-Ebene wurde das System als verteilte Microservice-Architektur konzipiert. Ziel war es, monolithische Strukturen aufzubrechen und einzelne fachliche Domänen (z. B. Produkte, Bestellungen, Zahlungen) in eigenständige, lose gekoppelte Services zu kapseln.

**Die zentralen fachlichen Bausteine und Services:**
1. **MeiShop (API Gateway):** Dient als zentraler Einstiegspunkt (Frontend/BFF) für externe Clients. Es orchestriert eingehende Anfragen und leitet sie an die jeweiligen Backend-Services weiter.
2. **ProductService & FtpProductCatalogService:** Verwalten den Produktkatalog. Während der `ProductService` auf einen lokalen In-Memory-Datastore zugreift, integriert der `FtpProductCatalogService` externe Produktlisten über ein simuliertes Datei-Backend. Zusätzlich bietet der **ProductODataService** komplexe Abfragemöglichkeiten (z. B. `$filter`, `$orderby`) nach dem OData v4 Standard.
3. **PaymentService & IEGEasyCreditcardService:** Verarbeiten finanzielle Transaktionen. Der `PaymentService` unterstützt Content Negotiation (JSON, XML, CSV) für B2B-Kunden. Echte Kreditkartenvalidierungen werden im Hintergrund via Round-Robin-Verfahren an mehrfach instanziierte `IEGEasyCreditcardService`-Knoten delegiert.
4. **OrderSagaService:** Koordiniert verteilte Transaktionen (Bestellvorgänge) über mehrere Microservices hinweg mittels des Saga-Patterns (Orchestrierung), um die Datenkonsistenz auch bei Ausfällen zu gewährleisten (kompensierende Transaktionen).
5. **WebhookSubscriberService:** Reagiert asynchron auf Ereignisse (z. B. abgeschlossene Zahlungen), die vom `PaymentService` via HTTP-Webhooks gesendet werden.

**Kommunikationsbeziehungen & Infrastruktur:**
- **Service Discovery:** Alle Services registrieren sich beim Start bei **HashiCorp Consul**. Das API Gateway ruft die Adressen dynamisch über Consul ab (Client-Side Discovery).
- **Logging:** Ein zentraler **LoggingService** nimmt Log-Nachrichten aller anderen Services entgegen. Um den Overhead zu minimieren und hohe Performance zu gewährleisten, erfolgt diese Querschnittskommunikation strikt über **gRPC** (HTTP/2).
- **Resilienz:** Das Gateway nutzt **Polly** für Retry-Mechanismen und Circuit Breaker bei Ausfällen von Backend-Services.

### 2. Mikro-Architektur (Deep Dive: PaymentService & OrderSagaService)

Auf der Mikro-Ebene folgen alle Services einem einheitlichen Aufbau, der eine klare Trennung von Anliegen (Separation of Concerns) sicherstellt.

**Beispiel 1: PaymentService**
- **Schnittstellen (Controllers):** Bietet REST-Endpunkte an (`/api/payments`). Durch die Implementierung eigener `InputFormatter` und `OutputFormatter` wird Content Negotiation unterstützt (CSV, XML, JSON). Zusätzlich gibt es einen `WebhookSubscriptionsController`, über den sich Clients für Events registrieren können.
- **Geschäftslogik (Services):** Kapselt Validierungen und die Weiterleitung an den Loadbalancer. Bei einer erfolgreichen Zahlung triggert der `WebhookService` asynchron alle registrierten Abonnenten über `HttpClient`-Aufrufe.
- **Datenhaltung (Repositories):** Die Persistenzschicht ist über ein `IPaymentRepository` abstrahiert. Dem Projekt-Constraint folgend wird eine `PaymentRepository`-Implementierung genutzt, die auf einer In-Memory `List<Payment>` basiert. 

**Beispiel 2: OrderSagaService**
- **Schnittstellen:** Nimmt über `/api/ordersaga/orders` Bestellanfragen entgegen.
- **Geschäftslogik (Orchestrator):** Dies ist das Herzstück des Services. Die Klasse `OrderSagaOrchestrator` implementiert eine State-Machine. Sie ruft sequenziell den `ProductService` auf (um die Warenverfügbarkeit zu prüfen) und danach den `PaymentService`. Schlägt ein Schritt fehl, ruft der Orchestrator aktiv Storno-Endpoints (kompensierende Transaktionen) der bereits erfolgreichen Services auf, um das System in einen konsistenten Zustand zurückzuführen.
- **Datenhaltung:** Der Status der verteilten Transaktion (`Pending`, `Confirmed`, `Cancelled`) wird in einem In-Memory-State-Store festgehalten.

### 3. Skalierbarkeit, Erweiterbarkeit und Austauschbarkeit

- **Erweiterbarkeit:** Durch die strenge Domänentrennung können neue Features leicht als separate Services hinzugefügt werden (wie es nachträglich mit dem `OrderSagaService` und `WebhookSubscriberService` geschehen ist), ohne bestehenden Code anzufassen.
- **Skalierbarkeit:** Das System ist horizontal skalierbar. Da Consul als Service Registry fungiert, können bei hoher Last (z. B. beim Payment) problemlos weitere Instanzen des `IEGEasyCreditcardService` hochgefahren werden. Das Gateway verteilt die Last per Round-Robin automatisch auf die neuen Knoten.
- **Austauschbarkeit:** Da alle Services über lose gekoppelte, sprachunabhängige REST/JSON-Schnittstellen oder gRPC kommunizieren, könnte jeder Service vollständig in einer anderen Programmiersprache (z. B. Go oder Java) neu geschrieben und nahtlos ausgetauscht werden. Auch die In-Memory-Datenhaltung kann dank des Repository-Patterns jederzeit durch z.B. Entity Framework Core + PostgreSQL ausgetauscht werden, ohne die Controller-Logik anzupassen.
