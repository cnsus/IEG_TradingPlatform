# Asynchrone Kommunikationsmuster – IEG Trading Platform

## 1. Ausgangslage

Die IEG Trading Platform nutzt aktuell **synchrone REST/HTTP-Kommunikation** zwischen allen Services. Das bedeutet: Client und Service muessen gleichzeitig verfuegbar sein, und der Aufrufer blockiert, bis die Antwort eintrifft.

**Probleme synchroner Kommunikation:**
- **Enge Laufzeitkopplung** – beide Seiten muessen gleichzeitig erreichbar sein
- **Kaskadierende Fehler** – ein langsamer Service blockiert die gesamte Kette
- **Kein natuerlicher Puffer** – Lastspitzen werden direkt weitergeleitet

> **Loesung:** Asynchrone Kommunikation ueber einen Message Broker entkoppelt Sender und Empfaenger zeitlich. Der Sender wartet nicht auf eine sofortige Antwort – der Empfaenger verarbeitet die Nachricht, sobald er bereit ist.

---

## 2. Asynchrone Kommunikationsstile

| Stil | Beschreibung | Beispiel in der Trading Platform |
|---|---|---|
| **Request/Response** | Sender erwartet zeitnah eine Antwort (ueber Reply-Queue) | Kreditkartentransaktion anstossen |
| **Notification** | Sender erwartet keine Antwort (Fire-and-Forget) | Bestellbestaetigung per E-Mail |
| **Request/Async Response** | Antwort kommt irgendwann (nicht sofort) | Bonitaetspruefung bei externem Anbieter |
| **Publish/Subscribe** | Event wird an beliebig viele Subscriber verteilt | `OrderCreated` → Payment, Inventory, Notification |
| **Publish/Async Response** | Event an mehrere Subscriber, einige antworten | Parallele Validierung (Zahlung + Lagerbestand) |

---

## 3. Technologien

| Kriterium | Apache Kafka | RabbitMQ |
|---|---|---|
| **Typ** | Event-Streaming-Plattform | Message Broker (AMQP) |
| **Durchsatz** | Sehr hoch | Hoch |
| **Replay** | Ja (Offset-basiert) | Nein |
| **.NET-Integration** | Confluent.Kafka | MassTransit / EasyNetQ |
| **Betriebskomplexitaet** | Hoch | Mittel |
| **Empfehlung** | Event Sourcing, hohe Volumina | Klassisches Messaging, RPC-Patterns |

---

## 4. Anwendung auf die Trading Platform

### Was bleibt synchron?
- Produkte abrufen (GET) – einfacher Lesezugriff, Ergebnis sofort benoetigt
- Akzeptierte Kreditkarten abrufen – Referenzdaten

### Was wird asynchron?
- **Bestellung aufgeben** → Publish/Subscribe (`OrderCreated`-Event)
- **Zahlung verarbeiten** → Request/Async Response
- **Bestellbestaetigung senden** → Notification (Fire-and-Forget)
- **Fehler loggen** → Notification (bereits als Fire-and-Forget implementiert via gRPC)

---

## 5. Vorteile und Herausforderungen

| Vorteile | Herausforderungen |
|---|---|
| Lose Laufzeitkopplung | Message Broker muss hochverfuegbar sein |
| Verbesserte Verfuegbarkeit (Broker puffert) | Eventual Consistency statt sofortiger Konsistenz |
| Unterstuetzt viele Kommunikationsmuster | Request/Reply-Stil wird komplexer |
| Unabhaengige Skalierung der Consumer | Idempotente Consumer erforderlich (at-least-once) |

---

## 6. Verwandte Patterns

- **Saga Pattern** – Verteilte Transaktionen ueber mehrere Services (z.B. Bestellung = Payment + Inventory)
- **CQRS** – Trennung von Schreib- und Leseoperationen, Events transportieren Aenderungen
- **Transactional Outbox** – Datenbankaenderung und Message-Versand atomar in einer Transaktion
- **Externalized Configuration** – Broker-Adressen und Queue-Namen ueber Konfiguration (z.B. Consul)
