# Aufgabe 1 (25 Punkte)

## a) Analyse & Cloud Deployment

#TODO

### Aufgabenstellung

Analyse: Machen Sie sich mit dem Ausgangs-Source-Code „SolTradingPlatform" vertraut. Publizieren Sie die beiden Services „MeiShop" und „IEGEasyCreditCardService" in die Microsoft Azure Cloud und testen Sie die Funktionalität. Alternativ können Sie die Projekte natürlich auch onpremise hosten (0 Punkte)

### Ausarbeitung


---

## b) Domain-Driven Design (DDD) im Zusammenhang mit Microservices

### Aufgabenstellung

Beschreiben Sie zuerst den Ansatz „Domain-Driven Design (DDD) im Zusammenhang mit Microservices. Überlegen Sie welche weiteren Microservices in Zusammenhang mit der Trading Platform sinnvoll wären. Beschreiben Sie danach die Funktionalitäten / Verantwortlichkeiten der einzelnen Microservices – Stichwort: Business Capabilities

### Ausarbeitung

### 1. Domain-Driven Design (DDD) und Microservices

#### 1.1 Grundidee von DDD

Domain-Driven Design stellt die **Fachdomaene** in den Mittelpunkt der Softwareentwicklung. Die wichtigsten Begriffe:

- **Domain:** Das Geschaeftsfeld – hier eine Online-Handelsplattform.
- **Ubiquitous Language:** Gemeinsame Fachsprache (*Product*, *Payment*, *Order*) in Code und Gespraech.
- **Bounded Context:** Abgegrenzter Bereich mit eigenem Modell und eigener Datenhoheit.
- **Entities / Value Objects:** Objekte mit Identitaet (Payment) bzw. ohne (Currency).

#### 1.2 Warum DDD und Microservices gut zusammenpassen

**Kernprinzip:** Ein Microservice = ein Bounded Context = eine Business Capability.

| DDD | Microservice |
|---|---|
| Bounded Context | Jeder Service hat eigenes Modell und eigene Daten |
| Ubiquitous Language | Fachbegriffe gelten innerhalb des Service |
| Lose Kopplung | Kommunikation nur ueber REST-APIs |
| Hohe Kohaesion | Jeder Service deckt genau eine Business Capability ab |

Der `ProductService` weiss nichts von Kreditkarten, der `IEGEasyCreditcardService` nichts von Produkten. Jeder Service ist autonom.

### 2. Microservices der Trading Platform

#### 2.1 Bestehende Services (Business Capabilities)

| Service | Port | Bounded Context | Verantwortlichkeit |
|---|---|---|---|
| **ProductService** | 7200 | Produktverwaltung | Liefert den Produktstamm (In-Memory) |
| **FtpProductCatalogService** | 7300 | Produktkatalog | Laedt Katalogdatei `products.txt` vom FTP-Server |
| **IEGEasyCreditcardService** | 7231 | Kreditkartenzahlung | Validiert Transaktionen, liefert Kartentypen, Health-Check |
| **PaymentService** | 7400 | Zahlungsabwicklung | Payments in JSON/XML/CSV via Content Negotiation |
| **MeiShop** | 7024 | Shop-Orchestrierung | API Gateway, aggregiert Backend-Services mit Polly-Retry |

#### 2.2 Sinnvolle Erweiterungen

- **OrderService** – Bestellprozess vom Warenkorb bis zur Bestaetigung
- **CustomerService** – Kundenstammdaten, Login, Lieferadressen
- **InventoryService** – Lagerbestand und Reservierungen
- **NotificationService** – E-Mail- und Versandbenachrichtigungen

---

## c) Detailbeschreibung Schnittstellen

#TODO

### Aufgabenstellung

Erstellen Sie eine Detailbeschreibung der angebotenen Schnittstellen inkl. Datenaustauschformate

### Ausarbeitung

#### ProductService

| Methode | Endpunkt | Response | Format |
|---|---|---|---|
| GET | `/api/products` | `["Laptop", "Smartphone", ...]` | JSON |

#### FtpProductCatalogService

| Methode | Endpunkt | Response | Format |
|---|---|---|---|
| GET | `/api/productcatalog` | Liste aus `products.txt` | JSON |

#### IEGEasyCreditcardService

| Methode | Endpunkt | Beschreibung | Format |
|---|---|---|---|
| GET | `/api/AcceptedCreditCards` | Akzeptierte Kartentypen | JSON |
| GET | `/api/CreditcardTransactions?id={id}` | Transaktion abfragen | JSON |
| POST | `/api/CreditcardTransactions` | Transaktion ausfuehren | JSON |
| GET | `/api/HealthCheck` | Consul Health-Probe | – |

**CreditcardTransaction:**
```json
{
  "creditcardNumber": "1234-5678-9012-3456",
  "creditcardType": "Visa",
  "amount": 99.99,
  "receiverName": "MeiShop GmbH"
}
```

#### PaymentService (Content Negotiation)

| Methode | Endpunkt | Beschreibung |
|---|---|---|
| GET | `/api/payments` | Alle Payments |
| GET | `/api/payments/{id}` | Einzelnes Payment |
| POST | `/api/payments` | Neues Payment anlegen |

Das Format wird ueber HTTP-Header gesteuert:
- **`Accept`** → Antwortformat
- **`Content-Type`** → Eingabeformat

Unterstuetzt: `application/json`, `application/xml`, `text/csv`.

**Beispiel – dasselbe Payment in drei Formaten:**

```json
{ "id": 1, "amount": 49.99, "currency": "EUR",
  "description": "Laptop Zubehoer", "paymentMethod": "CreditCard",
  "createdAt": "2026-01-15T00:00:00" }
```
```xml
<Payment><Id>1</Id><Amount>49.99</Amount><Currency>EUR</Currency>
<Description>Laptop Zubehoer</Description><PaymentMethod>CreditCard</PaymentMethod>
<CreatedAt>2026-01-15T00:00:00</CreatedAt></Payment>
```
```csv
Id,Amount,Currency,Description,PaymentMethod,CreatedAt
1,49.99,EUR,Laptop Zubehoer,CreditCard,2026-01-15
```

#### MeiShop (API Gateway)

| Methode | Endpunkt | Ruft auf | Resilience |
|---|---|---|---|
| GET | `/api/productlist` | ProductService | – |
| GET | `/api/productcatalog` | FtpProductCatalogService | – |
| GET | `/api/paymentmethods` | IEGEasyCreditcardService | Polly Retry (3× / 2s) |

#### Kommunikationsmuster

```
                          ┌──────────────┐
                          │    Client    │
                          │  (Browser)   │
                          └──────┬───────┘
                                 │ REST / JSON
                ┌────────────────┼────────────────┐
                │                │                │
                ▼                ▼                ▼
      ┌──────────────────┐  ┌──────────┐  ┌──────────────────┐
      │     MeiShop      │  │ Payment  │  │  (weitere        │
      │   API Gateway    │  │ Service  │  │   Services)      │
      │     :7024        │  │  :7400   │  │                  │
      │   + Polly Retry  │  │ JSON/XML │  │                  │
      └────────┬─────────┘  │  / CSV   │  └──────────────────┘
               │            └──────────┘
   ┌───────────┼───────────────────────────┐
   │           │                           │
   ▼           ▼                           ▼
┌─────────┐ ┌─────────────┐        ┌────────────────┐
│ Product │ │  FtpCatalog │        │  Creditcard    │
│ Service │ │   Service   │        │    Service     │
│  :7200  │ │    :7300    │        │     :7231      │
│in-memory│ │  FTP-Server │        │   in-memory    │
└─────────┘ └─────────────┘        └────────────────┘
```

Der Client ruft entweder den `PaymentService` direkt an oder geht ueber das Gateway `MeiShop`, das die Backend-Services orchestriert.

---

## d) Detailbeschreibung Datenhaltung

#TODO

### Aufgabenstellung

Erstellen Sie eine Detailbeschreibung der Datenhaltung – Stichwort: Decentralized Data Management

### Ausarbeitung

### Prinzip

> **Jeder Microservice verwaltet seine eigenen Daten. Es gibt kein gemeinsames Datenbankschema.**

- Jeder Service hat einen **eigenen Speicher**
- Kein direkter Zugriff auf fremde Daten
- Datenaustausch **nur ueber APIs**
- Services sind unabhaengig deploy- und skalierbar

### Speicher der einzelnen Services

| Service | Speicher | Beschreibung |
|---|---|---|
| ProductService | In-Memory Liste | 5 statische Produkte, Reset bei Neustart |
| FtpProductCatalogService | FTP-Server | Liest `products.txt` von localhost:21 |
| IEGEasyCreditcardService | In-Memory (hardcoded) | Kartentypen fix, Transaktionen nicht persistent |
| PaymentService | In-Memory Liste | 3 Beispiel-Payments, Laufzeit-Daten |
| MeiShop | – | Kein eigener Store, nur Aggregation |

### Vorteile

- **Polyglot Persistence:** Jeder Service waehlt die passende Technologie
- **Unabhaengige Skalierung** pro Service
- **Fehler-Isolation:** Ausfall einer Datenquelle betrifft nur einen Service
- **Team-Autonomie:** keine gemeinsamen Datenbank-Abhaengigkeiten

### Herausforderungen

- **Konsistenz** ueber Service-Grenzen → *Eventual Consistency*, *Saga Pattern*
- **Daten-Duplikation** ist gewollt – jeder Bounded Context hat sein eigenes Modell
- **Uebergreifende Abfragen** laufen ueber das API Gateway

### Beispiel: Bestellung aufgeben

```
1. Client  →  GET /api/products          →  ProductService
2. Client  →  GET /api/paymentmethods    →  MeiShop  →  CreditcardService
3. Client  →  POST /api/payments         →  PaymentService
              Content-Type: application/json
              { "amount": 999.99, "currency": "EUR", ... }
           ←  201 Created
```

Jeder Schritt trifft einen eigenen Service mit eigener Datenhaltung – die Orchestrierung uebernimmt der Client oder das Gateway.
