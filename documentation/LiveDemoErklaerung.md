# Erklärung der Live-Demo-Szenarien — IEG Trading Platform

Dieses Dokument erklärt einfach und verständlich, was bei jedem Schritt der Live-Demonstration im Hintergrund passiert, welche Konzepte dahinterstecken und warum diese Tests für das Gesamtsystem wichtig sind.

---

## Szenario 1: Dezentrale Datenhaltung (Local vs. FTP Catalog)
**Ziel des Szenarios:** Zeigen, dass das System Produktdaten aus völlig unterschiedlichen Quellen laden und über ein gemeinsames API-Gateway bereitstellen kann.

### 1. Lokaler Produktkatalog abrufen (`ProductService`)
*   **Befehl:**
    ```bash
    curl -k https://localhost:7200/api/Products
    ```
*   **Was passiert im Hintergrund?**  
    Wir fragen direkt den `ProductService` nach seinen Produkten. Dieser Service speichert seine Daten in einem lokalen Arbeitsspeicher-Repository (In-Memory). Er liefert uns direkt eine einfache Liste lokaler Produkte zurück (z. B. "Laptop", "Smartphone").
*   **Bedeutung:** Beweist, dass der primäre, lokale Produktkatalog voll funktionsfähig ist.

### 2. FTP-basierter Produktkatalog abrufen (`FtpProductCatalogService`)
*   **Befehl:**
    ```bash
    curl -k https://localhost:7300/api/ProductCatalog
    ```
*   **Was passiert im Hintergrund?**  
    Wir fragen den `FtpProductCatalogService`. Dieser Service liest Produktdaten aus einer Datei aus, die auf einem FTP-Server liegt (bzw. simuliert dies über einen FTP-Reader).
*   **Bedeutung:** Microservices müssen Daten nicht immer aus klassischen Datenbanken beziehen. Dieses Szenario zeigt, wie flexibel die Plattform ist und wie einfach sich **Legacy-Systeme** (Altsysteme, die Daten nur per FTP/Datei bereitstellen) integrieren lassen.

### 3. API Gateway Routing (`MeiShop` Gateway)
*   **Befehle:**
    *   Lokaler Katalog über Gateway: `curl -k https://localhost:7024/api/ProductList`
    *   FTP-Katalog über Gateway: `curl -k https://localhost:7024/api/ProductCatalog`
*   **Was passiert im Hintergrund?**  
    Der Client (z. B. eine Weboberfläche oder mobile App) schickt Anfragen nicht an die einzelnen Microservices, sondern an das zentrale Gateway **MeiShop** (Port 7024). Das Gateway nimmt die Anfrage entgegen, liest den Pfad aus (Routing) und leitet sie an den passenden Hintergrund-Service (`ProductService` oder `FtpProductCatalogService`) weiter.
*   **Bedeutung:** Das Gateway verbirgt die interne Struktur der Microservices vor dem Client. Es fungiert als **Single Point of Entry** (einzige Anlaufstelle), was Sicherheitsprüfungen, Logging und Routing extrem vereinfacht.

---

## Szenario 2: OData-Querying (Filtern & Sortieren)
**Ziel des Szenarios:** Demonstrieren, wie der Client mithilfe von OData v4 flexible und komplexe Abfragen (Filtern, Sortieren, Spaltenauswahl) durchführen kann, ohne dass die Entwickler für jeden Fall eine eigene API-Methode schreiben müssen.

> [!NOTE]
> *Hinweis für die Ausführung:* In modernen macOS-Terminals (z. B. mit `curl`) müssen Leerzeichen und Sonderzeichen in URLs codiert (`%20` statt Leerzeichen) oder in einfache Anführungszeichen gesetzt werden, da die URL sonst als fehlerhaft abgelehnt wird.

### 1. Filtern auf Produkte über 500 Euro
*   **Befehl:**
    ```bash
    curl -k 'https://localhost:7500/odata/Products?$filter=Price%20gt%20500'
    ```
*   **Was passiert im Hintergrund?**  
    Der OData-Service filtert die Produktdatenbank direkt auf dem Server. Die Abfrage `Price gt 500` steht für "Preis größer als (`gt` = greater than) 500". Es werden nur Produkte zurückgegeben, die dieses Kriterium erfüllen.
*   **Bedeutung:** Reduziert die Netzwerklast, da der Client nur die Daten erhält, die er wirklich benötigt.

### 2. Top 3 günstigste Produkte abrufen
*   **Befehl:**
    ```bash
    curl -k 'https://localhost:7500/odata/Products?$orderby=Price&$top=3'
    ```
*   **Was passiert im Hintergrund?**  
    Der Service sortiert (`$orderby`) alle Produkte aufsteigend nach ihrem Preis und liefert anschließend nur die obersten drei Einträge (`$top=3`) zurück.
*   **Bedeutung:** Perfekt geeignet für Features wie "Günstigste Angebote auf der Startseite anzeigen".

### 3. Nur bestimmte Spalten selektieren
*   **Befehl:**
    ```bash
    curl -k "https://localhost:7500/odata/Products?\$filter=Category%20eq%20'Laptops'&\$select=Name,Price"
    ```
*   **Was passiert im Hintergrund?**  
    Wir filtern nach Produkten in der Kategorie "Laptops" (`Category eq 'Laptops'`) und fordern mit `$select=Name,Price` explizit nur die Felder `Name` und `Price` an. Andere Daten wie ID, Beschreibung oder Lagerbestand werden gar nicht erst übertragen.
*   **Bedeutung:** Spart massiv Bandbreite und beschleunigt die Antwortzeit, was besonders bei Mobilgeräten mit schlechter Verbindung wichtig ist.

---

## Szenario 3: HTTP Content Negotiation
**Ziel des Szenarios:** Zeigen, dass ein einzelner API-Endpunkt dieselben Daten in völlig verschiedenen Formaten (JSON, XML, CSV) ausgeben kann, je nachdem, was der Client im HTTP-Header (`Accept`) anfordert.

### 1. Rückgabe als JSON
*   **Befehl:**
    ```bash
    curl -k -H "Accept: application/json" https://localhost:7400/api/Payments
    ```
*   **Was passiert im Hintergrund?**  
    Der Client signalisiert über `Accept: application/json`, dass er JSON wünscht. ASP.NET Core serialisiert die Zahlungen in das Standard-JSON-Format.
*   **Bedeutung:** JSON ist der Web-Standard für moderne Single-Page-Apps (z. B. Angular, React).

### 2. Rückgabe als XML
*   **Befehl:**
    ```bash
    curl -k -H "Accept: application/xml" https://localhost:7400/api/Payments
    ```
*   **Was passiert im Hintergrund?**  
    Durch `Accept: application/xml` formatiert der Server die Zahlungsdaten vollautomatisch in eine XML-Struktur um.
*   **Bedeutung:** Wichtig für die Kommunikation mit älteren Enterprise-Systemen (Legacy Java/C++-Anwendungen), die XML als Standard nutzen.

### 3. Rückgabe als CSV (Custom Formatter)
*   **Befehl:**
    ```bash
    curl -k -H "Accept: text/csv" https://localhost:7400/api/Payments
    ```
*   **Was passiert im Hintergrund?**  
    Der Client fordert das CSV-Format an. Da ASP.NET Core CSV nicht nativ unterstützt, greift ein im Projekt implementierter **Custom Output Formatter**. Dieser konvertiert die Zahlungsliste in eine tabellarische Textstruktur mit Kommas.
*   **Bedeutung:** Ermöglicht es dem Benutzer, Daten direkt per Klick in Excel zu importieren oder für Buchhaltungstools bereitzustellen.

---

## Szenario 4: Webhooks (Echtzeit-Benachrichtigung)
**Ziel des Szenarios:** Zeigen, wie ereignisgesteuerte Kommunikation (Event-Driven) zwischen Services funktioniert. Statt dass ein Empfänger-Service ständig beim Sender nachfragt ("Gibt es etwas Neues?"), benachrichtigt der Sender den Empfänger proaktiv, sobald ein Ereignis eintritt.

### 1. Webhook registrieren
*   **Befehl:**
    ```bash
    curl -k -X POST https://localhost:7400/api/WebhookSubscriptions \
      -H "Content-Type: application/json" \
      -d '{
        "CallbackUrl": "https://localhost:7600/api/Webhook/payment",
        "EventType": "payment.created"
      }'
    ```
*   **Was passiert im Hintergrund?**  
    Der `WebhookSubscriberService` (Port 7600) meldet sich beim `PaymentService` (Port 7400) für das Ereignis `payment.created` an und hinterlegt seine eigene Empfängeradresse (CallbackUrl).
*   **Bedeutung:** Ermöglicht lose Kopplung. Der `PaymentService` muss nicht wissen, was der Empfänger tut, sondern schickt einfach nur die Nachricht los.

### 2. Zahlung erzeugen (Ereignis auslösen)
*   **Befehl:**
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
*   **Was passiert im Hintergrund?**  
    Wir legen eine neue Zahlung im `PaymentService` an. Sobald die Zahlung gespeichert ist, feuert der Service das `payment.created`-Event. Er sucht in seiner Datenbank nach aktiven Webhook-Abonnenten für dieses Event und sendet die Zahlungsdaten per HTTP-POST an deren Callback-URL.
*   **Bedeutung:** Das fachliche Ereignis ("Zahlung erfolgt") stößt die Benachrichtigungskette an.

### 3. Empfang prüfen
*   **Befehl:**
    ```bash
    curl -k https://localhost:7600/api/Webhook/notifications
    ```
*   **Was passiert im Hintergrund?**  
    Wir fragen den Empfänger-Service ab. Wir sehen, dass die Benachrichtigung über das soeben erstellte Payment dort in Echtzeit eingegangen ist.
*   **Bedeutung:** Zeigt die erfolgreiche, asynchrone Echtzeit-Kommunikation zwischen den Microservices.

---

## Szenario 5: Resilience, Load Balancing & gRPC Logging
**Ziel des Szenarios:** Demonstrieren, wie ausfallsicher das Gesamtsystem aufgebaut ist. Fällt ein wichtiger Service aus, fängt das API-Gateway dies unbemerkt ab (Resilience), verteilt die Last auf gesunde Server (Load Balancing) und meldet Fehler zentral zur Diagnose (gRPC Logging).

### 1. Resilienten Aufruf durchführen
*   **Befehl:**
    ```bash
    curl -k https://localhost:7024/api/PaymentMethods
    ```
*   **Was passiert im Hintergrund?**  
    Das Gateway `MeiShop` fragt die akzeptierten Kreditkarten ab. Im Hintergrund laufen drei Instanzen des `IEGEasyCreditcardService` (Ports 7231, 7232 und 7233). `MeiShop` nutzt **Round-Robin Load Balancing**: Es wechselt bei jeder Anfrage die Ziel-Instanz ab, um die Last gleichmäßig zu verteilen.
*   **Bedeutung:** Ausgeglichene Serverauslastung und Vermeidung von Überlastung.

### 2. Chaos Step (Instanz beenden)
*   **Aktion:**  
    Wir beenden manuell eine der Kreditkarten-Instanzen (z. B. Port 7231).
*   **Was passiert im Hintergrund?**  
    Wir simulieren einen echten Serverausfall im Live-Betrieb.

### 3. Aufruf wiederholen (Polly-Resilience & Failover testen)
*   **Befehl:**
    ```bash
    curl -k https://localhost:7024/api/PaymentMethods
    ```
*   **Was passiert im Hintergrund?**  
    Wenn das Gateway versucht, die abgestürzte Instanz (Port 7231) aufzurufen, scheitert die Verbindung.
    *   **Der Rettungs-Mechanismus (Polly):** Eine im Gateway hinterlegte Policy (Polly) versucht es automatisch erneut (Retry). Scheitert auch der 4. Versuch, führt das System ein **Failover** durch: Es schaltet auf die nächste gesunde Instanz (z. B. Port 7232) um. Der Aufruf gelingt und der Client bekommt die Daten, ohne eine Fehlermeldung zu sehen.
*   **Bedeutung:** **High Availability (Hochverfügbarkeit)**. Einzelne Serverausfälle führen nicht zum Systemabsturz oder Fehlern beim Kunden.

### 4. Zentrales Logging via gRPC
*   **Log-Pfad:** `src/LoggingService/bin/Debug/net10.0/logs/error_log.txt`
*   **Was passiert im Hintergrund?**  
    Während des gescheiterten Verbindungsaufbaus zu Port 7231 hat das Gateway jeden einzelnen Fehlversuch sofort über ein extrem schnelles Kommunikationsprotokoll (**gRPC**) an den zentralen `LoggingService` übertragen. Dieser speichert die Fehler strukturiert in einer Textdatei ab.
*   **Bedeutung:** In verteilten Microservice-Systemen ist es schwer, Fehler aufzuspüren. Dieses Szenario beweist, dass alle Systemfehler an einer **zentralen Stelle gesammelt werden (Observability)**, damit Administratoren sie sofort analysieren können.

---

## Szenario 6: SAGA Pattern & Kompensierende Transaktionen
**Ziel des Szenarios:** Datensicherheit über Servicegrenzen hinweg. Da Microservices eigene Datenbanken haben, können wir keine klassische Datenbank-Transaktion nutzen. Wenn ein Schritt einer mehrstufigen Bestellung fehlschlägt, müssen alle vorherigen Schritte automatisch rückgängig gemacht werden (Kompensation / Rollback).

### 1. Erfolgreiche Saga (Bestellung bestätigt)
*   **Befehl:**
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
*   **Was passiert im Hintergrund?**  
    Der `OrderSagaOrchestrator` steuert die Transaktion schrittweise über verschiedene Microservices hinweg:
    1.  **Order anlegen:** Bestellung wird im `OrderSagaService` im Status `Pending` angelegt. (Erfolgreich)
    2.  **Produkt prüfen:** Der `ProductService` bestätigt, dass das Produkt "Laptop" vorrätig ist. (Erfolgreich)
    3.  **Zahlung erstellen:** Der `PaymentService` bucht den Betrag ab. (Erfolgreich)
    4.  **Order bestätigen:** Der Saga-Status wechselt auf `Confirmed` (Bestätigt).
*   **Bedeutung:** Das ist der "Happy Path" – die Bestellung wird erfolgreich abgeschlossen und bezahlt.

### 2. Fehlgeschlagene Saga mit Kompensation (Rollback)
*   **Befehl:**
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
*   **Was passiert im Hintergrund?**  
    Wir fordern eine Bestellung für ein nicht existierendes Produkt an:
    1.  **Order anlegen:** Die Bestellung wird im Status `Pending` angelegt. (Erfolgreich)
    2.  **Produkt prüfen:** Der `ProductService` sucht das Produkt und meldet einen Fehler: "Produkt existiert nicht". (Fehlgeschlagen!)
    3.  **Die Kompensation:** Der Orchestrator bricht den Ablauf ab. Da Schritt 2 fehlgeschlagen ist, darf die Bestellung nicht aktiv bleiben. Der Orchestrator startet automatisch die Kompensationsaktion: Er storniert die zuvor angelegte Bestellung im `OrderSagaService` (Status wechselt auf `Cancelled` / `Storniert`).
*   **Bedeutung:** Verhindert Dateninkonsistenzen (Datenmüll). Es stellt sicher, dass kein Geld für nicht lieferbare Produkte abgebucht wird und keine ungültigen Bestellungen im System verbleiben.
