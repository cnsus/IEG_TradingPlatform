# Live Demo Runbook — IEG Trading Platform

Dieses Dokument dient als Leitfaden für die Durchführung der Live-Demonstration der Microservice-Plattform. Es führt Schritt für Schritt durch die wichtigsten implementierten Features der Aufgaben 1–10 und der SAGA-Bonusaufgabe.

---

## 1. Voraussetzungen & Starten der Services

Um die Demo vorzubereiten, müssen Consul und die Anwendungs-Services gestartet sein.

### A. Consul (Service Discovery) starten
Starte den Consul-Agenten in einem separaten Terminal-Fenster im Root-Verzeichnis des Projekts:
```bash
consul agent -dev -config-dir=Consul/config
```
*   **Web-UI zur Kontrolle:** [http://localhost:8500/ui](http://localhost:8500/ui) (hier sind die registrierten Instanzen des `IEGEasyCreditcardService` sichtbar).

### B. Microservices starten
Starte alle Microservices in einem zweiten Terminal-Fenster:
```bash
chmod +x start-all.sh
./start-all.sh
```

---

## 2. Port- & Linkübersicht für die Demo

Nutze diese Tabelle, um während der Präsentation schnell die Weboberflächen (Swagger UI) oder Endpunkte aufzurufen:

| Service | Port (HTTPS / HTTP) | Link / Swagger UI |
| :--- | :--- | :--- |
| **MeiShop** (Gateway) | `7024` / `5009` | [https://localhost:7024/swagger/index.html](https://localhost:7024/swagger/index.html) |
| **Consul** | N/A / `8500` | [http://localhost:8500/ui](http://localhost:8500/ui) |
| **ProductService** | `7200` / `5200` | [https://localhost:7200/swagger/index.html](https://localhost:7200/swagger/index.html) |
| **FtpProductCatalogService**| `7300` / `5300` | [https://localhost:7300/swagger/index.html](https://localhost:7300/swagger/index.html) |
| **PaymentService** | `7400` / `5400` | [https://localhost:7400/swagger/index.html](https://localhost:7400/swagger/index.html) |
| **ProductODataService** | `7500` / `5501` | [https://localhost:7500/odata/Products](https://localhost:7500/odata/Products) |
| **WebhookSubscriberService**| `7600` / `5601` | [https://localhost:7600/swagger/index.html](https://localhost:7600/swagger/index.html) |
| **OrderSagaService** | `7700` / `5701` | [https://localhost:7700/swagger/index.html](https://localhost:7700/swagger/index.html) |
| **LoggingService** (gRPC) | N/A / `5500` | *Nur gRPC (Kein HTTP-Interface)* |
| **CreditcardService #1** | `7231` / `5228` | [https://localhost:7231/swagger/index.html](https://localhost:7231/swagger/index.html) |
| **CreditcardService #2** | `7232` / `5229` | [https://localhost:7232/swagger/index.html](https://localhost:7232/swagger/index.html) |
| **CreditcardService #3** | `7233` / `5230` | [https://localhost:7233/swagger/index.html](https://localhost:7233/swagger/index.html) |

---

## 3. Demo-Szenarien Schritt für Schritt

### Szenario 1: Dezentrale Datenhaltung (Local vs. FTP Catalog)
Zeige, dass Services Daten aus unterschiedlichen Quellen bereitstellen und das API-Gateway diese bündelt.

1.  **Lokaler Produktkatalog (In-Memory Repository):**
    ```bash
    curl -k https://localhost:7200/api/Products
    ```
2.  **FTP-basierter Produktkatalog (FTP Reader):**
    ```bash
    curl -k https://localhost:7300/api/ProductCatalog
    ```
3.  **API Gateway Routing (MeiShop):**
    Zeige im Browser/Swagger, wie MeiShop die beiden Kataloge unter `/api/ProductList` und `/api/ProductCatalog` anspricht.

---

### Szenario 2: OData-Querying (Filtern & Sortieren)
Demonstriere die OData v4 Integration im `ProductODataService`.

1.  **Filtern auf Produkte über 500 Euro:**
    ```bash
    curl -k "https://localhost:7500/odata/Products?\$filter=Price gt 500"
    ```
2.  **Top 3 günstigste Produkte abrufen:**
    ```bash
    curl -k "https://localhost:7500/odata/Products?\$orderby=Price&\$top=3"
    ```
3.  **Nur bestimmte Spalten selektieren:**
    ```bash
    curl -k "https://localhost:7500/odata/Products?\$filter=Category eq 'Laptops'&\$select=Name,Price"
    ```

---

### Szenario 3: HTTP Content Negotiation
Zeige, dass der `PaymentService` je nach `Accept`-Header Daten unterschiedlich serialisiert.

1.  **Rückgabe als JSON:**
    ```bash
    curl -k -H "Accept: application/json" https://localhost:7400/api/Payments
    ```
2.  **Rückgabe als XML:**
    ```bash
    curl -k -H "Accept: application/xml" https://localhost:7400/api/Payments
    ```
3.  **Rückgabe als CSV (Custom Formatter):**
    ```bash
    curl -k -H "Accept: text/csv" https://localhost:7400/api/Payments
    ```

---

### Szenario 4: Webhooks (Echtzeit-Benachrichtigung)
Demonstriere ereignisgesteuerte Kommunikation zwischen Diensten.

1.  **Webhook registrieren:**
    Melde den `WebhookSubscriberService` beim `PaymentService` an:
    ```bash
    curl -k -X POST https://localhost:7400/api/WebhookSubscriptions \
      -H "Content-Type: application/json" \
      -d '{
        "CallbackUrl": "https://localhost:7600/api/Webhook/payment",
        "EventType": "payment.created"
      }'
    ```
2.  **Zahlung erzeugen:**
    ```bash
    curl -k -X POST https://localhost:7400/api/Payments \
      -H "Content-Type: application/json" \
      -d '{
        "Vendor": "Handelsplattform Shop",
        "CustomerCreditCardnumber": "1234-5678-9012-3456",
        "Amount": 150.00,
        "OrderId": 101
      }'
    ```
3.  **Empfang prüfen:**
    Überprüfe, ob die Benachrichtigung sofort beim Subscriber eingegangen ist:
    ```bash
    curl -k https://localhost:7600/api/Webhook/notifications
    ```

---

### Szenario 5: Resilience, Load Balancing & gRPC Logging
Demonstriere Ausfallsicherheit (Polly Retry + Round-Robin Load Balancing) und zentrales Logging.

1.  **Zahlungsmethoden abrufen (Load Balancing):**
    Führe einen Aufruf über MeiShop aus, welcher an die registrierten Creditcard-Instanzen weiterleitet (Round-Robin):
    ```bash
    curl -k https://localhost:7024/api/PaymentMethods
    ```
2.  **Chaos Step (Instanz beenden):**
    Stoppe die erste Creditcard-Instanz (Port `7231`) manuell im Terminal (z. B. durch `Ctrl+C` oder Task-Kill).
3.  **Aufruf wiederholen (Resilience & Failover):**
    Führe denselben `GET`-Befehl erneut aus. Er läuft trotz des Ausfalls ohne Fehler durch!
    *   *Erklärung:* MeiShop bemerkt den Fehler, startet die Polly-Retry-Logik (versucht es 4x auf Port 7231) und wechselt (Failover) erfolgreich auf die nächste gesunde Instanz (Port `7232`).
4.  **Zentrales Logging (gRPC):**
    Zeige, dass der Verbindungsfehler via gRPC an den `LoggingService` gesendet wurde und in der Datei `logs/error_log.txt` dokumentiert ist.

---

### Szenario 6: SAGA Pattern & Kompensierende Transaktionen
Demonstriere die Transaktionssicherheit über Microservice-Grenzen hinweg.

1.  **Erfolgreiche Saga (Bestellung bestätigt):**
    ```bash
    curl -k -X POST https://localhost:7700/api/OrderSaga/place \
      -H "Content-Type: application/json" \
      -d '{
        "ProductName": "Laptop",
        "Amount": 1,
        "CustomerCreditCardnumber": "1111-2222-3333-4444",
        "Vendor": "FH Shop"
      }'
    ```
    *Erwartetes Ergebnis:* Status `Confirmed`, SagaOutcome `ERFOLGREICH`.

2.  **Fehlgeschlagene Saga mit Kompensation (Rollback):**
    Führe eine Bestellung mit einem ungültigen Produktnamen oder einer zu hohen Menge aus, um einen Fehler im Checkout-Prozess zu provozieren (z. B. Produkt existiert nicht):
    ```bash
    curl -k -X POST https://localhost:7700/api/OrderSaga/place \
      -H "Content-Type: application/json" \
      -d '{
        "ProductName": "NichtExistierendesProdukt",
        "Amount": 10,
        "CustomerCreditCardnumber": "1111-2222-3333-4444",
        "Vendor": "FH Shop"
      }'
    ```
    *Erwartetes Ergebnis:* HTTP Status `409 Conflict`, SagaOutcome `FEHLGESCHLAGEN (Kompensation durchgefuehrt)`. Alle bereits reservierten Schritte werden rückgängig gemacht.
