# Aufgabe 9 - SAGA-Pattern (10 Punkte)

> [!IMPORTANT]
> **Constraint:** Above all, after completing this assignment, it is important to verify that the software still works.

## Aufgabenstellung

Machen Sie sich mit dem Begriff SAGA-Pattern vertraut. Überlegen und implementieren Sie ein mögliches SAGA-Pattern Szenario (Service & Client)-Szenario. Umgang mit Ausfallsicherheit – Stichwort: Design for failure / Resilient Software Design (10 Punkte)

## Ausarbeitung

### Was ist das SAGA-Pattern?

Das **SAGA-Pattern** ist ein Entwurfsmuster fuer verteilte Systeme, das Datenkonsistenz ueber mehrere Microservices hinweg sicherstellt — ohne auf verteilte Transaktionen (z.B. Two-Phase-Commit / 2PC) angewiesen zu sein.

In einer Microservice-Architektur besitzt jeder Service seinen eigenen Datastore (Decentralized Data Management). Eine klassische ACID-Transaktion, die mehrere Services umfasst, ist daher nicht moeglich. Das SAGA-Pattern loest dieses Problem, indem es eine Geschaeftstransaktion als **Kette von lokalen Transaktionen** modelliert.

#### Kernkonzepte

- **Lokale Transaktionen:** Jeder Saga-Schritt fuehrt eine Transaktion innerhalb seines eigenen Service aus und triggert anschliessend den naechsten Schritt.
- **Kompensierende Transaktionen:** Falls ein Schritt fehlschlaegt, werden zuvor abgeschlossene Schritte durch kompensierende Transaktionen rueckgaengig gemacht. Eine Kompensation ist dabei keine technische Undo-Operation, sondern eine **semantische Umkehroperation** (z.B. Bestellung stornieren statt loeschen).
- **Eventual Consistency:** Da keine globalen Sperren verwendet werden, garantiert das SAGA-Pattern **Eventual Consistency** statt sofortiger ACID-Atomizitaet.

### Zwei Implementierungsansaetze

| Eigenschaft      | Choreographie (dezentral)          | Orchestrierung (zentral)               |
|------------------|------------------------------------|----------------------------------------|
| Koordination     | Ereignisgesteuert (Pub/Sub)        | Zentraler Orchestrator sendet Befehle  |
| Sichtbarkeit     | Schwer nachzuvollziehen            | Klare, zentrale Zustandsverfolgung     |
| Kopplung         | Lose (keine zentrale Steuerung)    | Abhaengigkeit vom Orchestrator         |
| Geeignet fuer    | Einfache Workflows, wenige Services| Komplexe Workflows, viele Services     |
| Ausfallrisiko    | Kein Single Point of Failure       | Orchestrator als potenzieller SPOF     |

**Choreographie:** Jeder Service reagiert auf Events und publiziert eigene Events fuer den naechsten Schritt. Es gibt keinen zentralen Koordinator.

**Orchestrierung:** Ein zentraler Saga-Orchestrator steuert den gesamten Ablauf, indem er Befehle an die beteiligten Services sendet und deren Antworten auswertet. Bei Fehler loest er die Kompensation aus.

### Projektbezug: most wanTED

Fuer das Projekt wurde der **Orchestrierungs-Ansatz** gewaehlt, da dieser eine klare Nachvollziehbarkeit des Saga-Ablaufs bietet und besser zu der bestehenden synchronen REST-Kommunikation der Plattform passt.

### Implementiertes Szenario: Bestellvorgang (Order Saga)

Der `OrderSagaService` (Port 7700) fungiert als Saga-Orchestrator und koordiniert einen Bestellvorgang ueber zwei bestehende Services:

```
┌──────────────────────────────────────────────────────┐
│                 OrderSagaService                      │
│               (Saga-Orchestrator)                     │
│                   :7700                               │
│                                                       │
│  Schritt 1: Order anlegen (lokal, Status: Pending)    │
│       │                                               │
│       ▼                                               │
│  Schritt 2: Produktverfuegbarkeit pruefen ──────────► ProductService :7200
│       │                                               │
│       ▼                                               │
│  Schritt 3: Zahlung erstellen ──────────────────────► PaymentService :7400
│       │                                               │
│       ▼                                               │
│  Schritt 4: Order bestaetigen (Status: Confirmed)     │
│                                                       │
│  BEI FEHLER:                                          │
│  Kompensation: Order stornieren (Status: Cancelled)   │
└──────────────────────────────────────────────────────┘
```

#### Saga-Schritte im Detail

| Schritt | Aktion                          | Service         | Kompensation bei Fehler        |
|---------|----------------------------------|-----------------|--------------------------------|
| 1       | Order anlegen (Status: Pending) | OrderSagaService| —                              |
| 2       | Produktverfuegbarkeit pruefen   | ProductService  | Order stornieren (Cancelled)   |
| 3       | Zahlung erstellen               | PaymentService  | Order stornieren (Cancelled)   |
| 4       | Order bestaetigen (Confirmed)   | OrderSagaService| —                              |

#### Zustandsautomat der Order

```
[Pending] ──► [ProductChecked] ──► [PaymentCreated] ──► [Confirmed]
    │                │                    │
    └────────────────┴────────────────────┘
                     │
                     ▼
               [Cancelled]
          (Kompensation durchgefuehrt)
```

### Architektur des OrderSagaService

```
Controllers/
  OrderSagaController.cs        → REST-API (Bestellung aufgeben, Orders abfragen)
Models/
  Order.cs                       → Bestellungs-Modell mit Status-Enum
  PlaceOrderRequest.cs           → Eingabe-DTO fuer neue Bestellungen
  SagaStep.cs                    → Protokollierung der einzelnen Saga-Schritte
Services/
  IOrderRepository.cs            → Interface (Repository Pattern)
  OrderRepository.cs             → In-Memory Speicher fuer Bestellungen
  IOrderSagaOrchestrator.cs      → Interface fuer den Saga-Orchestrator
  OrderSagaOrchestrator.cs       → Kernlogik: Orchestrierung + Kompensation
Program.cs                       → Konfiguration und Service-Registrierung
```

### API-Endpunkte

| Methode | Endpunkt                   | Beschreibung                                      |
|---------|----------------------------|----------------------------------------------------|
| POST    | `/api/ordersaga/place`     | Startet eine neue Bestell-Saga                     |
| GET     | `/api/ordersaga/orders`    | Zeigt alle Bestellungen mit Status an              |
| GET     | `/api/ordersaga/orders/{id}`| Zeigt eine einzelne Bestellung                    |

### Testen des SAGA-Szenarios

**Voraussetzung:** ProductService (:7200) und PaymentService (:7400) muessen laufen.

**Erfolgreicher Fall** (Produkt existiert im Katalog):
```bash
curl -X POST https://localhost:7700/api/ordersaga/place \
  -H "Content-Type: application/json" \
  -d '{"productName": "Laptop", "amount": 999.99, "currency": "EUR", "customerName": "Max Mustermann", "paymentMethod": "CreditCard"}' \
  -k
```
→ Ergebnis: Order mit Status `Confirmed`, alle 4 Saga-Schritte erfolgreich.

**Fehlschlag mit Kompensation** (Produkt existiert NICHT):
```bash
curl -X POST https://localhost:7700/api/ordersaga/place \
  -H "Content-Type: application/json" \
  -d '{"productName": "NichtExistierendesProdukt", "amount": 50.00, "currency": "EUR", "customerName": "Max Mustermann", "paymentMethod": "CreditCard"}' \
  -k
```
→ Ergebnis: Order mit Status `Cancelled`, Schritt 2 fehlgeschlagen, Kompensation dokumentiert.

**Fehlschlag bei Service-Ausfall** (PaymentService nicht erreichbar):
```bash
# PaymentService stoppen, dann Bestellung ausfuehren
curl -X POST https://localhost:7700/api/ordersaga/place \
  -H "Content-Type: application/json" \
  -d '{"productName": "Laptop", "amount": 999.99, "currency": "EUR", "customerName": "Max Mustermann", "paymentMethod": "CreditCard"}' \
  -k
```
→ Ergebnis: Order mit Status `Cancelled`, Schritt 3 fehlgeschlagen, Kompensation dokumentiert.

### Design for Failure / Resilient Software Design

Das implementierte Szenario demonstriert folgende Prinzipien der Ausfallsicherheit:

1. **Graceful Degradation:** Falls ein Service nicht erreichbar ist, wird die Bestellung nicht in einem inkonsistenten Zustand belassen, sondern sauber storniert.
2. **Kompensierende Transaktionen:** Jeder Fehler fuehrt zu einer dokumentierten Kompensation, die den Systemzustand konsistent haelt.
3. **Transparente Fehlerdokumentation:** Jeder Saga-Schritt wird protokolliert (SagaStep), sodass der Grund eines Fehlschlags jederzeit nachvollziehbar ist.
4. **Idempotenz:** Die Kompensation (Stornierung) kann mehrfach ausgefuehrt werden, ohne den Zustand zu verfaelschen.
