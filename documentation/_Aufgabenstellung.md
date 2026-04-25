# Projektaufgaben - IEG Trading Platform

---

## Aufgabe 1 (25 Punkte)

- [ ] **a)** Analyse des Ausgangs-Source-Codes "SolTradingPlatform". Services "MeiShop" und "IEGEasyCreditCardService" in Azure Cloud publizieren (oder on-premise hosten) und Funktionalität testen.
- [ ] **b)** Domain-Driven Design (DDD) im Zusammenhang mit Microservices beschreiben. Weitere sinnvolle Microservices identifizieren. Funktionalitäten/Verantwortlichkeiten der einzelnen Microservices beschreiben (Business Capabilities).
- [ ] Detailbeschreibung der angebotenen Schnittstellen inkl. Datenaustauschformate erstellen.
- [ ] Detailbeschreibung der Datenhaltung erstellen (Decentralized Data Management).

---

## Aufgabe 2 (10 Punkte)

- [ ] Microservice Produktkatalog 1: Liste von Produkten aus einem "microservice local datastore" anbieten (Decentralized Data Management).
- [ ] Hard codierte Werte im MeiShop/ProductList-Controller durch Aufruf des neuen Services ersetzen.
- [ ] Microservice Produktkatalog 2: Produkte aus einem Text File auf einem FTP-Server (oder anderem Persistence Store) auslesen und zur Verfügung stellen.

---

## Aufgabe 3 (10 Punkte)

- [ ] "IEGEasyCreditCardService" mehrfach publizieren und im Round-Robin-Stil aufrufen (Skalierung).
- [ ] Retry-Logik bei Fehlern implementieren.
- [ ] Zentrales Logging-Service (gRPC) für Fehlerprotokollierung implementieren.
- [ ] Nach n erfolglosen Versuchen das nächste Service aufrufen (Ausfallsicherheit).
- [ ] Geeignetes Framework und Skalierungsmöglichkeiten recherchieren und ggf. einsetzen.

---

## Aufgabe 4 (10 Punkte)

- [ ] Theoretische Überlegungen zum Einsatz von asynchronen Kommunikationsstilen in der Handelsplattform (Pattern: Messaging).

---

## Aufgabe 5 (10 Punkte)

- [ ] Zusätzliches Payment-Service schreiben, das JSON, XML und CSV verarbeiten und erzeugen kann (HTTP Content Negotiation).

---

## Aufgabe 6 (10 Punkte)

- [ ] Theoretische Überlegungen zu einem PaymentService-Broker (Vermittlung zwischen Shops und Payment-Services).
- [ ] Zusätzliche Patterns und Quellen recherchieren (Broker Pattern, Message Broker, Canonical Data Model).

---

## Aufgabe 7 (10 Punkte)

- [ ] Webhook-Szenario überlegen und implementieren.

---

## Aufgabe 8 (10 Punkte)

- [ ] OData-Szenario überlegen und implementieren (Service & Client).

---

## Aufgabe 9 (10 Punkte)

- [ ] SAGA-Pattern Szenario überlegen und implementieren (Service & Client).
- [ ] Ausfallsicherheit beschreiben (Design for Failure / Resilient Software Design).

---

## Aufgabe 10 (10 Punkte)

- [ ] "Open Data" beschreiben und mögliche Anwendungsfälle im Zusammenhang mit der Handelsplattform erläutern.

---

---

# TED Aufgaben

---

## TED 1 - Fachartikelanalyse (8 Punkte)

- [ ] Einen der bereitgestellten Fachartikel auswählen und analysieren.
- [ ] Zentrale Argumentation erläutern und Beitrag zur Konzeption/Gestaltung von Microservice-Architekturen beschreiben.
- [ ] Erkenntnisse auf die Projektarbeit übertragen und Nutzen, Grenzen und Implikationen reflektieren.

---

## TED 2 - Makro- und Mikro-Architektur (8 Punkte)

- [ ] Makro-Architektur: Zentrale fachliche Bausteine, abgeleitete Microservices und Kommunikationsbeziehungen darstellen.
- [ ] Mikro-Architektur: Inneren Aufbau von 1-2 Microservices beschreiben (Schnittstellen, Geschäftslogik, Datenhaltung).
- [ ] Begründung bzgl. Erweiterbarkeit, Austauschbarkeit und Skalierbarkeit.

---

## TED 3 - Domain Driven Design (8 Punkte)

- [ ] Bounded Contexts herausarbeiten und Beziehungen erläutern (4 Punkte).
- [ ] DDD-Konzepte einsetzen oder bewusst nicht verwenden begründen (Shared Kernel, Customer/Supplier, Conformist, Anticorruption Layer, Separate Ways, Open Host Service, Published Language).
- [ ] Strukturierte Darstellung: Aufgaben der Microservices, Kommunikationswege/-stile, ausgetauschte Daten/Formate, Datenhaltung.
- [ ] Zwei alternative Ansätze zu Domain Driven Design beschreiben (max. 1/2 Seite) (4 Punkte).

---

## TED 4 - Implementierung I: Discovery & Configuration (12 Punkte)

- [ ] Microservice implementieren, das in mehreren Instanzen deployt werden kann.
- [ ] Konsumierendes Microservice implementieren, das verfügbare Instanzen dynamisch ermittelt.
- [ ] Service-Discovery-Lösung einsetzen (z.B. HashiCorp Consul).
- [ ] Zentralen Konfigurationsdienst realisieren.
- [ ] Mindestens einmal das Sidecar-Pattern einsetzen und dessen Funktion erläutern.

---

## TED 5 - Implementierung II: Secrets (12 Punkte)

- [ ] Microservice implementieren, das Secrets/Tokens für Kommunikation benötigt.
- [ ] Sichere Verwaltung und Bereitstellung von Secrets beschreiben (z.B. HashiCorp Vault, Azure Key Vault).
- [ ] Dienst zur Verwaltung von Secrets implementieren/verwenden.
- [ ] Begriffe erläutern: API Key, SAML, OAuth, OpenID Connect (Authentifizierung, Autorisierung, sichere Service-Kommunikation).
- [ ] Mindestens einmal das Sidecar-Pattern einsetzen und dessen Rolle erläutern.

---

## TED 6 - Implementierung III: Asynchrones Messaging & Content Negotiation (12 Punkte)

- [ ] Microservice mit asynchroner/ereignisgetriebener Kommunikation implementieren.
- [ ] Möglichkeiten sowie Vor-/Nachteile beschreiben.
- [ ] Eine Variante konkret einsetzen: Queue (Point-to-Point), Topic (Publish/Subscribe) oder WebHooks.
- [ ] Content Negotiation implementieren und deren Rolle erläutern.

---

## TED 7 - Qualität & Monitoring (12 Punkte)

- [ ] Konzept zur Qualitätssicherung entwickeln (Integrationstests, Last-/Performanztests).
- [ ] System-, Application- und Business-Metriken beschreiben und begründen.
- [ ] Zentralen Logging- und Tracing-Ansatz entwickeln (End-to-End-Tracing).
- [ ] Observability erläutern (Logs, Metriken, Traces) und Maßnahmen zur Verbesserung aufzeigen.

---

## TED 8 - Implementierung: Alternative "Produktempfehlung" (12 Punkte)

- [ ] Kostenpflichtige Produktplatzierung via SOAP-Endpunkt implementieren.
- [ ] Genehmigungsprozess mit Workflow-Engine modellieren, ausführen und überwachen.
- [ ] Prozessschritte, Zustandsübergänge, beteiligte Rollen und Integration beschreiben.
- [ ] Business Process Modelling und BPEL erläutern.
- [ ] BPEL-Ansatz mit moderner Orchestrierung/Choreographie vergleichen.

---

## TED 9 - KI, Low-Code & Visionäre Weiterentwicklung (12 Punkte)

- [ ] **a)** KI-gestützte Unterstützung eines Geschäftsprozesses beschreiben (3 Punkte).
- [ ] **b)** Low-Code-/No-Code-basierte Umsetzung erläutern (3 Punkte).
- [ ] **c)** Visionäre Weiterentwicklung der Architektur: 2 Themenbereiche ausführlicher beschreiben (Discovery & Configuration, Logging, Messaging, Secrets Management, Observability, resiliente Softwareentwicklung) (3 Punkte).
- [ ] Ergänzend: Agentic AI, RPA und ein agentisches Framework (z.B. OpenClaw) beschreiben (max. 1 Seite) (3 Punkte).

---

## TED 10 - Aufbereitung und Präsentation (20 Punkte)

- [ ] Verständliche, durchdachte und schlüssige schriftliche Dokumentation erstellen.
- [ ] Kompakte und sachliche Aufbereitung der Ergebnisse.
- [ ] Präsentation mit fachlicher Einordnung und nachvollziehbarer Demonstration.

---

## TED 11 - Funktionierende Gesamtlösung (8 Punkte)

- [ ] Alle Komponenten zu einer nachvollziehbaren und demonstrierbaren Gesamtlösung zusammenführen.
- [ ] Zusammenspiel der Bausteine, Integrationsbeziehungen und Kommunikationswege demonstrieren.

---

---

# BONUS-Punkte (max. 5 Punkte - eines auswählen)

- [ ] **A)** Saga-Pattern: 1 Microservice mit Saga Pattern implementieren + 2PC beschreiben (5 Punkte).
- [ ] **B)** Cloud-Service: 1 Microservice das ein fremdes Cloud-Service konsumiert (5 Punkte).
- [ ] **C)** CQRS-Pattern: 1 Microservice mit CQRS Pattern implementieren (5 Punkte).
- [ ] **D)** Event Sourcing: Pattern Event Sourcing implementieren (5 Punkte).
- [ ] **E)** Deployment-Szenario: Geeignetes Deployment-Szenario überlegen und implementieren (5 Punkte).
