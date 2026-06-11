# BONUS-Punkte (max. 5 Punkte - eines auswählen)

## A) Einsatz des Saga-Patterns (5 Punkte)

Implementieren Sie konkret 1 Microservice welches das Saga Pattern verwendet (als Ersatz für Distributed Transactions). Beschreiben Sie in diesem Zusammenhang auch das Protokoll 2PC – two-phase commit. https://microservices.io/patterns/data/saga.html

## B) Konsumieren eines beliebigen Service aus der „Cloud" (5 Punkte)

Implementieren Sie konkret 1 Microservice welches ein beliebiges „fremdes" (Cloud)-Service verwendet

## C) Einsatz des CQRS Patterns (5 Punkte)

Implementieren Sie konkret 1 Microservice welches das CQRS Pattern verwendet

## D) Einsatz von Event Sourcing (5 Punkte)

Implementieren Sie konkret das Pattern Event Sourcing

## E) Überlegen Sie sich ein geeignetes Deployment-Szenario (5 Punkte)

Implementieren Sie konkret ein geeignetes Deployment-Szenario

## Ausarbeitung

### Option A) Einsatz des Saga-Patterns (Erledigt in Aufgabe 9)

Die praktische Implementierung des Saga-Patterns wurde bereits im Rahmen von **Aufgabe 9** umgesetzt. Dort wurde der `OrderSagaService` als Orchestrator entwickelt, der einen verteilten Bestellvorgang ueber den `ProductService` und den `PaymentService` hinweg steuert und bei Fehlern kompensierende Transaktionen ausfuehrt (siehe `documentation/Aufgaben/Aufgabe9.md`).

#### Das Protokoll 2PC (Two-Phase Commit)

In klassischen verteilten Datenbanksystemen wird zur Sicherstellung von ACID-Eigenschaften (Atomicity, Consistency, Isolation, Durability) ueber mehrere Knoten hinweg oft das **Two-Phase Commit (2PC) Protokoll** verwendet. Es stellt sicher, dass eine verteilte Transaktion entweder von allen beteiligten Systemen vollstaendig committet oder ueberall verworfen (Rollback) wird.

Das Protokoll laeuft in zwei Phasen ab, gesteuert von einem zentralen Koordinator:

1. **Prepare Phase (Voting Phase):** Der Koordinator fragt alle beteiligten Datenbanken/Services, ob sie bereit sind, die Transaktion durchzufuehren. Alle Teilnehmer sperren (Lock) die benoetigten Ressourcen und antworten mit "Yes" (bereit) oder "No" (Fehler/Konflikt).
2. **Commit Phase (Decision Phase):** 
   - Wenn *alle* Teilnehmer mit "Yes" geantwortet haben, sendet der Koordinator den "Commit"-Befehl an alle. Die Transaktion wird ueberall dauerhaft gespeichert.
   - Wenn *auch nur ein einziger* Teilnehmer mit "No" antwortet (oder ein Timeout auftritt), sendet der Koordinator einen "Abort/Rollback"-Befehl an alle. Die Transaktion wird ueberall abgebrochen.

**Warum Sagas statt 2PC in Microservices?**
2PC hat schwerwiegende Nachteile in modernen Microservice-Architekturen:
- **Blockierend:** Waehrend der Prepare-Phase muessen Ressourcen ueber das Netzwerk hinweg gesperrt bleiben, was die Skalierbarkeit massiv einschraenkt.
- **CAP-Theorem:** Bei Netzwerkpartitionen (Microservice nicht erreichbar) blockiert 2PC die Verfuegbarkeit des gesamten Systems.
- **Fehlende Unterstuetzung:** Viele moderne NoSQL-Datenbanken und Message-Broker unterstuetzen kein 2PC (XA-Transaktionen).

Aus diesen Gruenden wird in Microservice-Architekturen stattdessen das in Aufgabe 9 implementierte **Saga-Pattern** verwendet, das auf langanhaltenden Datenbank-Sperren verzichtet und stattdessen auf Eventual Consistency und kompensierende Transaktionen setzt.
