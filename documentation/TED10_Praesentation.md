# TED 10 - Aufbereitung und Präsentation (20 Punkte)

## Aufgabenstellung

Die Präsentation dient nicht nur der Darstellung, sondern auch der fachlichen Einordnung und der nachvollziehbaren Demonstration der erarbeiteten Lösung. Ebenso werden eine verständliche, durchdachte und schlüssige schriftliche Dokumentation sowie eine kompakte und sachliche Aufbereitung der Ergebnisse als wesentliche Bestandteile der Projektarbeit betrachtet. Entscheidend sind nicht Umfang, Mindestseitenzahlen oder die bloße Aneinanderreihung bzw. Übernahme von Inhalten, sondern die fachliche Qualität, eigenständige Strukturierung, Nachvollziehbarkeit und Konsistenz der Ausarbeitung. Wird keine Präsentation durchgeführt, können die entsprechenden Punkte für Präsentation, Demonstration und verteidigende Erläuterung nicht erreicht werden.

## Ausarbeitung: Demo-Skript

Für die Live-Demonstration im Rahmen der Präsentation wurde folgendes "Happy-Path" Demo-Skript konzipiert, welches alle Kernelemente der Microservice-Architektur zeigt:

### Vorbereitung
1. **Consul starten:** Sicherstellen, dass Consul lokal auf Port `8500` läuft.
2. **Services hochfahren:** Ausführen von `./start-all.sh` (oder `.bat`). Warten, bis alle 11 .NET-Prozesse gestartet sind.

### Schritt 1: Architektur-Setup & Discovery beweisen
- **Aktion:** Consul-Dashboard im Browser (`http://localhost:8500`) öffnen.
- **Moderation:** "Hier sehen wir, dass sich alle Microservices erfolgreich registriert haben. Besonders hervorzuheben sind die 3 Instanzen des `IEGEasyCreditcardService`, die für das Round-Robin Load-Balancing bereitstehen."

### Schritt 2: OData & API-Gateway
- **Aktion:** Über Postman oder den Browser einen OData-Request absenden: `https://localhost:7500/api/products?$filter=Price lt 50&$orderby=Name`
- **Moderation:** "Unser ProductODataService erlaubt komplexe Abfragen, wie wir hier sehen. Für den normalen Shop-Traffic nutzen wir jedoch unser API-Gateway (`MeiShop` auf Port 7024), das als zentraler Einstiegspunkt fungiert und die Backend-Komplexität verbirgt."

### Schritt 3: Der SAGA-Bestellprozess (The Core)
- **Aktion:** Eine Bestellung über Swagger am `OrderSagaService` (`https://localhost:7700/swagger`) absetzen. (POST `/api/ordersaga/orders` mit gültiger Produkt-ID und Preis).
- **Moderation:** "Jetzt demonstrieren wir verteilte Transaktionen. Der Saga-Orchestrator nimmt die Bestellung an, reserviert das Produkt beim `ProductService` und stößt anschließend den `PaymentService` an. Das Payment wird an einen der 3 Creditcard-Knoten delegiert."

### Schritt 4: Webhooks & Observability
- **Aktion:** Die Konsole des `WebhookSubscriberService` oder dessen Logs öffnen.
- **Moderation:** "Weil die Zahlung im Saga-Prozess erfolgreich war, hat der PaymentService sofort asynchron einen Webhook gefeuert. Hier im Subscriber sehen wir den Eingang der Bestätigung. Parallel dazu wurden sämtliche Systemzustände per High-Performance gRPC an unseren zentralen `LoggingService` übertragen."

*(Optionaler Schritt 5: Simulation eines Fehlers)*
- **Aktion:** Den `PaymentService` manuell beenden und eine neue SAGA-Bestellung absetzen.
- **Moderation:** "Das Gateway und der Saga-Orchestrator bemerken den Ausfall. Polly-Policies greifen (Circuit Breaker), und der Saga-Orchestrator sendet sofort eine Kompensation (Storno) an den ProductService, um den Lagerbestand nicht dauerhaft zu blockieren."
