# Domain-Driven Design (DDD) und Microservices – IEG Trading Platform

## 1. Domain-Driven Design im Kontext von Microservices

### 1.1 Was ist Domain-Driven Design?

Domain-Driven Design (DDD) ist ein Ansatz zur Softwareentwicklung, bei dem die **Fachdomaene** (Business Domain) im Mittelpunkt steht. Eric Evans praegte diesen Begriff und definierte zentrale Konzepte:

- **Domain (Fachdomaene):** Das Geschaeftsfeld, in dem die Software eingesetzt wird – hier: eine Online-Handelsplattform.
- **Ubiquitous Language:** Eine gemeinsame Sprache zwischen Entwicklern und Fachexperten. Begriffe wie *Payment*, *Product*, *Order* oder *CreditcardTransaction* werden einheitlich im Code und in Gespraechen verwendet.
- **Bounded Context:** Ein klar abgegrenzter Bereich innerhalb der Domaene, in dem ein bestimmtes Modell gueltig ist. Jeder Bounded Context hat seine eigene Definition von Begriffen und seine eigene Datenhoheit.
- **Entities und Value Objects:** Entities haben eine eindeutige Identitaet (z.B. ein Payment mit Id), Value Objects sind durch ihre Attribute definiert (z.B. eine Waehrung).
- **Aggregates:** Cluster von Entities und Value Objects, die als Einheit behandelt werden.

### 1.2 Warum DDD und Microservices zusammenpassen

DDD und Microservices ergaenzen sich ideal:

| DDD-Konzept | Microservice-Umsetzung |
|---|---|
| **Bounded Context** | Jeder Microservice bildet einen Bounded Context ab. Er hat seine eigene Domaenenlogik, seine eigenen Modelle und seine eigene Datenhaltung. |
| **Ubiquitous Language** | Innerhalb eines Microservice wird die Fachsprache des jeweiligen Kontexts verwendet (z.B. spricht der PaymentService von *Amount*, *Currency*, *PaymentMethod*). |
| **Lose Kopplung** | Microservices kommunizieren nur ueber definierte Schnittstellen (REST APIs). Interne Modelle werden nicht nach aussen exponiert. |
| **Hohe Kohaesion** | Jeder Service buendelt genau die Funktionalitaet, die zu seiner Fachdomaene gehoert (Business Capability). |

**Kernprinzip:** Ein Microservice = ein Bounded Context = eine Business Capability.

Das bedeutet konkret: Der `ProductService` weiss nichts ueber Kreditkartentransaktionen, und der `IEGEasyCreditcardService` kennt keine Produktdaten. Jeder Service ist autonom und verantwortlich fuer seinen fachlichen Bereich.

### 1.3 Strategisches Design der Trading Platform

Die IEG Trading Platform laesst sich in folgende Subdomaenen aufteilen:

```
Trading Platform (Core Domain)
├── Product Management        (Supporting Subdomain)
├── Product Catalog            (Supporting Subdomain)
├── Payment Processing         (Core Subdomain)
├── Order Management           (Core Subdomain)       [Vorschlag]
├── Customer Management        (Supporting Subdomain)  [Vorschlag]
├── Inventory / Lagerhaltung   (Supporting Subdomain)  [Vorschlag]
├── Notification               (Generic Subdomain)     [Vorschlag]
└── API Gateway / Orchestrator (Supporting Subdomain)
```

---

## 2. Microservices der Trading Platform

### 2.1 Bestehende Microservices

#### ProductService
- **Bounded Context:** Produktverwaltung
- **Business Capability:** Verwaltung des Produktstamms (Laptop, Smartphone, Tablet, etc.)
- **Port:** https://localhost:7200
- **Verantwortlichkeit:** Bereitstellung der verfuegbaren Produkte aus einem lokalen In-Memory-Datenspeicher

#### FtpProductCatalogService
- **Bounded Context:** Produktkatalog (externe Quelle)
- **Business Capability:** Beschaffung von Produktkatalogdaten aus einem externen FTP-Server
- **Port:** https://localhost:7300
- **Verantwortlichkeit:** Asynchrones Laden des Produktkatalogs (`products.txt`) von einem FTP-Server und Bereitstellung als REST-API

#### IEGEasyCreditcardService
- **Bounded Context:** Kreditkartenzahlung
- **Business Capability:** Validierung und Verarbeitung von Kreditkartentransaktionen
- **Port:** https://localhost:7231
- **Verantwortlichkeit:** Akzeptierte Kartentypen bereitstellen, Transaktionen validieren, Health-Check fuer Service Discovery (Consul)

#### PaymentService
- **Bounded Context:** Zahlungsabwicklung
- **Business Capability:** Verwaltung und Verarbeitung von Zahlungen in verschiedenen Formaten (JSON, XML, CSV) mittels HTTP Content Negotiation
- **Port:** https://localhost:7400
- **Verantwortlichkeit:** Payments erstellen und abfragen, Content Negotiation ueber `Accept`- und `Content-Type`-Header

#### MeiShop (API Gateway)
- **Bounded Context:** Shop-Orchestrierung
- **Business Capability:** Aggregation und Weiterleitung von Anfragen an Backend-Services
- **Port:** https://localhost:7024
- **Verantwortlichkeit:** API Gateway Pattern – ruft ProductService, FtpProductCatalogService und IEGEasyCreditcardService auf. Implementiert Resilience-Pattern mit Polly (Retry-Policy)

### 2.2 Vorgeschlagene weitere Microservices

#### OrderService (Vorschlag)
- **Bounded Context:** Bestellverwaltung
- **Business Capability:** Verwaltung des gesamten Bestellprozesses – von der Warenkorbanlage bis zur Bestellbestaetigung
- **Typische Funktionen:**
  - Warenkorb erstellen und verwalten
  - Bestellung aufgeben (orchestriert Payment und Inventory)
  - Bestellstatus abfragen
  - Bestellhistorie pro Kunde

#### CustomerService (Vorschlag)
- **Bounded Context:** Kundenverwaltung
- **Business Capability:** Verwaltung von Kundenstammdaten und Authentifizierung
- **Typische Funktionen:**
  - Kundenregistrierung und Login
  - Kundenprofil verwalten (Name, Adresse, E-Mail)
  - Lieferadressen verwalten

#### InventoryService (Vorschlag)
- **Bounded Context:** Lagerverwaltung
- **Business Capability:** Bestandsfuehrung und Verfuegbarkeitspruefung
- **Typische Funktionen:**
  - Lagerbestand pro Produkt abfragen
  - Bestand reservieren bei Bestelleingang
  - Bestand aktualisieren bei Wareneingang

#### NotificationService (Vorschlag)
- **Bounded Context:** Benachrichtigungen
- **Business Capability:** Versand von Benachrichtigungen an Kunden und interne Systeme
- **Typische Funktionen:**
  - Bestellbestaetigung per E-Mail
  - Versandbenachrichtigung
  - Zahlungsbestaetigung

---

## 3. Schnittstellen und Datenaustauschformate

### 3.1 Schnittstellenuebersicht

#### ProductService

| Methode | Endpunkt | Beschreibung | Request-Body | Response-Body | Format |
|---------|----------|-------------|--------------|---------------|--------|
| GET | `/api/products` | Alle Produkte abrufen | – | `["Laptop", "Smartphone", ...]` | JSON |

#### FtpProductCatalogService

| Methode | Endpunkt | Beschreibung | Request-Body | Response-Body | Format |
|---------|----------|-------------|--------------|---------------|--------|
| GET | `/api/productcatalog` | Produktkatalog vom FTP laden | – | `["Produkt1", "Produkt2", ...]` | JSON |

#### IEGEasyCreditcardService

| Methode | Endpunkt | Beschreibung | Request-Body | Response-Body | Format |
|---------|----------|-------------|--------------|---------------|--------|
| GET | `/api/AcceptedCreditCards` | Akzeptierte Kartentypen | – | `["American", "Diners", "Master", "Visa", "Blue Monday"]` | JSON |
| GET | `/api/CreditcardTransactions?id={id}` | Transaktion abfragen | – | `"value{id}"` | JSON |
| POST | `/api/CreditcardTransactions` | Transaktion ausfuehren | `CreditcardTransaction` (siehe unten) | `201 Created` oder `400 Bad Request` | JSON |
| GET/HEAD | `/api/HealthCheck` | Health-Probe fuer Consul | – | `200 OK` | – |

**CreditcardTransaction (Model):**
```json
{
  "creditcardNumber": "1234-5678-9012-3456",
  "creditcardType": "Visa",
  "amount": 99.99,
  "receiverName": "MeiShop GmbH"
}
```

#### PaymentService (mit Content Negotiation)

| Methode | Endpunkt | Beschreibung | Request-Body | Response-Body | Formate |
|---------|----------|-------------|--------------|---------------|---------|
| GET | `/api/payments` | Alle Payments abrufen | – | Liste von Payments | JSON, XML, CSV |
| GET | `/api/payments/{id}` | Einzelnes Payment | – | Ein Payment | JSON, XML, CSV |
| POST | `/api/payments` | Neues Payment anlegen | Ein Payment | Erstelltes Payment | JSON, XML, CSV |

**Content Negotiation – Steuerung ueber HTTP-Header:**

Der Client bestimmt das gewuenschte Format ueber Standard-HTTP-Header:

- **`Accept`-Header** steuert das **Antwortformat** (Response)
- **`Content-Type`-Header** steuert das **Eingabeformat** (Request Body)

| Accept / Content-Type Header | Format | Beispiel |
|------------------------------|--------|---------|
| `application/json` | JSON | Standard-Format |
| `application/xml` | XML | Fuer XML-basierte Systeme |
| `text/csv` | CSV | Fuer Tabellenkalkulationen / Datenaustausch |

**Payment-Objekt in allen drei Formaten:**

JSON (`Accept: application/json`):
```json
{
  "id": 1,
  "amount": 49.99,
  "currency": "EUR",
  "description": "Laptop Zubehoer",
  "paymentMethod": "CreditCard",
  "createdAt": "2026-01-15T00:00:00"
}
```

XML (`Accept: application/xml`):
```xml
<Payment>
  <Id>1</Id>
  <Amount>49.99</Amount>
  <Currency>EUR</Currency>
  <Description>Laptop Zubehoer</Description>
  <PaymentMethod>CreditCard</PaymentMethod>
  <CreatedAt>2026-01-15T00:00:00</CreatedAt>
</Payment>
```

CSV (`Accept: text/csv`):
```csv
Id,Amount,Currency,Description,PaymentMethod,CreatedAt
1,49.99,EUR,Laptop Zubehoer,CreditCard,2026-01-15
```

#### MeiShop (API Gateway)

| Methode | Endpunkt | Ruft auf | Resilience |
|---------|----------|----------|------------|
| GET | `/api/productlist` | ProductService (`https://localhost:7200/api/products`) | – |
| GET | `/api/productcatalog` | FtpProductCatalogService (`https://localhost:7300/api/productcatalog`) | – |
| GET | `/api/paymentmethods` | IEGEasyCreditcardService (`https://localhost:7231/api/AcceptedCreditCards`) | Polly Retry: 3 Versuche, 2s Wartezeit |

### 3.2 Kommunikationsmuster

```
┌──────────┐     REST/JSON      ┌──────────────────┐
│  Client   │◄──────────────────►│     MeiShop      │
│ (Browser) │                    │  (API Gateway)   │
└──────────┘                    └────────┬─────────┘
                                         │
                    ┌────────────────────┼────────────────────┐
                    │                    │                    │
                    ▼                    ▼                    ▼
          ┌─────────────────┐  ┌─────────────────┐  ┌──────────────────┐
          │ ProductService  │  │ FtpCatalogSvc   │  │ CreditcardSvc    │
          │ :7200           │  │ :7300           │  │ :7231            │
          │ [In-Memory]     │  │ [FTP-Server]    │  │ [In-Memory]      │
          └─────────────────┘  └─────────────────┘  └──────────────────┘

          ┌──────────────────┐
          │ PaymentService   │◄── Direkt vom Client aufrufbar
          │ :7400            │    JSON / XML / CSV via Content Negotiation
          │ [In-Memory]      │
          └──────────────────┘
```

---

## 4. Datenhaltung – Decentralized Data Management

### 4.1 Prinzip

Ein zentrales Prinzip der Microservice-Architektur ist **Decentralized Data Management** (dezentrale Datenverwaltung):

> **Jeder Microservice verwaltet seine eigenen Daten – es gibt kein geteiltes Datenbankschema.**

Das bedeutet:
- Jeder Service hat seinen **eigenen Datenspeicher** (Datenbank, In-Memory-Store, Dateisystem, externer Service)
- Kein Service greift **direkt** auf die Daten eines anderen Services zu
- Datenaustausch erfolgt **ausschliesslich ueber APIs** (REST-Schnittstellen)
- Jeder Service ist **autonom** und kann unabhaengig deployed und skaliert werden

### 4.2 Datenhaltung der einzelnen Services

| Microservice | Speichertyp | Datenmodell | Beschreibung |
|---|---|---|---|
| **ProductService** | In-Memory (`List<string>`) | Produktnamen | Statische Liste mit 5 Produkten. Jeder Neustart setzt die Daten zurueck. Einfachste Form der dezentralen Datenhaltung. |
| **FtpProductCatalogService** | Externer FTP-Server | Textdatei (`products.txt`) | Liest Produktkatalog asynchron von einem FTP-Server (localhost:21). Die Daten liegen ausserhalb des Service – der Service agiert als Adapter. |
| **IEGEasyCreditcardService** | In-Memory (hardcoded) | Akzeptierte Kartentypen, CreditcardTransaction | Kartentypen sind im Controller hardcodiert. Transaktionen werden validiert, aber nicht persistent gespeichert. |
| **PaymentService** | In-Memory (`List<Payment>`) | Payment (Id, Amount, Currency, Description, PaymentMethod, CreatedAt) | In-Memory-Liste mit 3 Beispiel-Payments. Neue Payments werden zur Laufzeit hinzugefuegt, gehen bei Neustart verloren. |
| **MeiShop** | Kein eigener Store | – | Als API Gateway haelt MeiShop keine eigenen Daten. Er aggregiert Daten der Backend-Services ueber HTTP-Aufrufe. |

### 4.3 Vorteile der dezentralen Datenhaltung

1. **Technologiefreiheit (Polyglot Persistence):** Jeder Service kann die fuer ihn optimale Speichertechnologie waehlen – der ProductService nutzt eine In-Memory-Liste, der FtpProductCatalogService liest von FTP, ein zukuenftiger OrderService koennte eine relationale Datenbank verwenden.

2. **Unabhaengige Skalierung:** Services mit hoher Last (z.B. ProductService) koennen unabhaengig skaliert werden, ohne die Datenhaltung anderer Services zu beeinflussen.

3. **Fehler-Isolation:** Faellt die Datenquelle eines Services aus (z.B. FTP-Server nicht erreichbar), sind die anderen Services nicht betroffen. MeiShop faengt solche Fehler ueber Resilience-Pattern (Polly Retry) ab.

4. **Autonomie:** Teams koennen ihre Services unabhaengig weiterentwickeln, da es keine Abhaengigkeiten auf Datenbankebene gibt.

### 4.4 Herausforderungen

- **Datenkonsistenz:** Da es keine zentrale Datenbank gibt, muss Konsistenz ueber Services hinweg durch Patterns wie *Eventual Consistency* oder *Saga Pattern* sichergestellt werden.
- **Daten-Duplikation:** Verschiedene Services koennen aehnliche Daten halten (z.B. Produktname im OrderService und ProductService). Das ist gewollt – jeder Bounded Context hat sein eigenes Modell.
- **Abfragen ueber Service-Grenzen:** Komplexe Abfragen, die Daten mehrerer Services benoetigen, muessen ueber den API Gateway (MeiShop) orchestriert werden.

### 4.5 Datenfluss-Beispiel: Bestellung aufgeben

```
1. Client ──GET /api/products──► ProductService
   Client ◄── ["Laptop", "Smartphone", ...] ──

2. Client ──GET /api/paymentmethods──► MeiShop ──► CreditcardService
   Client ◄── ["Visa", "Master", ...] ──

3. Client ──POST /api/payments──► PaymentService
   (Content-Type: application/json)
   { "amount": 999.99, "currency": "EUR", "paymentMethod": "Visa", ... }
   Client ◄── 201 Created ──
```

Jeder Schritt adressiert einen eigenen Service mit eigener Datenhaltung. Die Services wissen nichts voneinander – die Orchestrierung liegt beim Client oder beim API Gateway.
