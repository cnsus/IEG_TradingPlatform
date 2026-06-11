# TED 8 - Implementierung: Alternative „Produktempfehlung" (12 Punkte)

## Aufgabenstellung

Ergänzend zur umfragebasierten Entscheidungsunterstützung soll im Projekt „most wanTED" auch die Möglichkeit vorgesehen werden, eine besonders prominente Produktplatzierung kostenpflichtig zu beantragen. Hierzu wird die Spezifikation des zu bewerbenden Produkts an einen SOAP-Endpunkt übermittelt. Die eingehende Promotionsanfrage durchläuft anschließend einen festgelegten Genehmigungsprozess mit mehreren fachlichen Entscheidungsschritten. Am Ende des Prozesses wird die Produktpromotion in Abhängigkeit von Produktpreis, Produktbeschreibung und der Freigabe durch die Gesellschafterinnen und Gesellschafter genehmigt oder abgelehnt. Beschreiben Sie, wie dieser Prozess mit Hilfe einer Workflow-Engine modelliert, ausgeführt und überwacht werden kann. Gehen Sie dabei insbesondere auf Prozessschritte, Zustandsübergänge, beteiligte Rollen sowie die Integration in Ihre bestehende Microservice-Architektur ein. Erläutern Sie außerdem die Begriffe Business Process Modelling und BPEL und ordnen Sie diese fachlich in den beschriebenen Anwendungsfall ein. Vergleichen Sie den BPEL-Ansatz mit modernen Formen der Orchestrierung und Choreographie von Microservices und reflektieren Sie deren jeweilige Eignung für die „most wanTED"-Lösung.

## Ausarbeitung

### 1. Modellierung und Prozessschritte via Workflow-Engine

Der Genehmigungsprozess für kostenpflichtige Produktplatzierungen wird als Workflow modelliert, um den Status und langlaufende Freigaben (die asynchron und manuell erfolgen) sauber tracken zu können.

**Prozessschritte & Zustandsübergänge:**
1. **Trigger:** Ein externer B2B-Kunde sendet eine XML-Payload (Produktspezifikation) an unseren neu zu schaffenden **SOAP-Endpunkt** (`/soap/promotion-requests`).
2. **Status "Eingegangen":** Der Service validiert die Payload und startet eine Instanz in der Workflow-Engine (z.B. *Elsa Workflows* oder *Camunda*). 
3. **Automatisierte Prüfung (Service-Task):** Die Workflow-Engine ruft synchron den `ProductService` auf, um den Produktpreis und Bestand abzugleichen. Liegt der Preis unter einer festgelegten Schwelle, geht der Status auf **"Automatisch Abgelehnt"**.
4. **Manuelle Freigabe (Human-Task):** Erfüllt das Produkt die Kriterien, wechselt der Zustand auf **"Wartet auf Freigabe"**. Ein Gesellschafter (Rolle: *Approver*) erhält eine Aufgabe im Workflow-Dashboard.
5. **Abschluss:** Nach dem manuellen Review wird der Status entweder auf **"Genehmigt"** (System triggert den `PaymentService` für die Rechnungstellung) oder **"Abgelehnt"** gesetzt.

**Integration in die Architektur:**
Die Workflow-Engine agiert als eigenständiger Microservice. Sie kommuniziert via REST/JSON mit den bestehenden Services (`ProductService`, `PaymentService`) und bietet nach außen den geforderten SOAP-Endpunkt für Altsysteme der Werbepartner an.

### 2. Business Process Modelling (BPM) und BPEL

- **BPM (Business Process Modelling):** Beschreibt die Methodik, Geschäftsprozesse grafisch oder textuell so abzubilden, dass sie von Fachabteilungen verstanden und von Systemen ausgeführt werden können (z.B. mittels BPMN 2.0).
- **BPEL (Business Process Execution Language):** Ein in der SOA-Ära (Service-Oriented Architecture) etablierter, auf XML basierender Standard zur Orchestrierung von Webservices (hauptsächlich SOAP). BPEL führt Logik top-down aus und verknüpft verschiedene Webservices zu einem Gesamtablauf. 
*Einordnung:* Im vorliegenden Szenario wäre BPEL technisch in der Lage, den SOAP-Endpunkt anzubieten und die nachfolgenden Prüfschritte zu orchestrieren. Allerdings ist BPEL für komplexe, asynchrone "Human-Tasks" oft zu starr.

### 3. Vergleich: BPEL vs. moderne Orchestrierung/Choreographie

| Kriterium | BPEL (SOA) | Moderne Orchestrierung (z.B. Saga-Pattern) | Choreographie (Event-Driven) |
|---|---|---|---|
| **Paradigma** | Zentrale Steuerung, stark gekoppelt an SOAP/XML. | Zentraler Orchestrator steuert via REST/gRPC oder Events. | Dezentral; Services reagieren autark auf Events (Kafka/RabbitMQ). |
| **Kopplung** | Sehr eng. Der Orchestrator muss alle XML-Schnittstellen kennen. | Lose. Der Orchestrator steuert den Ablauf, aber Schnittstellen sind leichtgewichtig (JSON). | Sehr lose. Niemand kennt den Gesamtprozess, alle reagieren nur. |
| **Fehlerbehandlung** | Komplexe Fehlerbehandlung durch verteilte XA-Transaktionen. | **Compensating Transactions** (Gegenbuchungen bei Fehlern). | Komplex, da der Fehler über asynchrone Events zurückverfolgt werden muss. |

**Reflexion für "most wanTED":**
Für die "most wanTED" Architektur ist BPEL heutzutage **nicht mehr zeitgemäß**. Wir setzen auf REST, gRPC und OData anstelle von schwerfälligen SOAP-XML-Stacks. 
Wie wir bei der Implementierung des `OrderSagaService` (Aufgabe 9) demonstriert haben, eignet sich eine **moderne Orchestrierung mittels Saga-Pattern** viel besser für unsere Lösung. Ein Saga-Orchestrator ist leichtgewichtig, skaliert im Container-Umfeld besser und löst das Problem von Ausfällen eleganter über *Compensating Transactions* statt über blockierende verteilte Transaktionen. Für den Workflow mit *Human-Tasks* wäre eine leichtgewichtige Engine wie *Elsa* (direkt in .NET integrierbar) der Choreographie oder einem alten BPEL-Server definitiv vorzuziehen.
