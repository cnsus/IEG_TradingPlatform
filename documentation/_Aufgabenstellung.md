# Projektaufgaben - IEG Trading Platform

---

## Aufgabe 1 (25 Punkte)

- [ ] **a)** Analyse: Machen Sie sich mit dem Ausgangs-Source-Code „SolTradingPlatform" vertraut. Publizieren Sie die beiden Services „MeiShop" und „IEGEasyCreditCardService" in die Microsoft Azure Cloud und testen Sie die Funktionalität. Alternativ können Sie die Projekte natürlich auch onpremise hosten (0 Punkte)

- [x] **b)** Beschreiben Sie zuerst den Ansatz „Domain-Driven Design (DDD) im Zusammenhang mit Microservices. Überlegen Sie welche weiteren Microservices in Zusammenhang mit der Trading Platform sinnvoll wären. Beschreiben Sie danach die Funktionalitäten / Verantwortlichkeiten der einzelnen Microservices – Stichwort: Business Capabilities

- [ ] Erstellen Sie eine Detailbeschreibung der angebotenen Schnittstellen inkl. Datenaustauschformate

- [ ] Erstellen Sie eine Detailbeschreibung der Datenhaltung – Stichwort: Decentralized Data Management

---

## Aufgabe 2 (10 Punkte)

- [x] Erstellen Sie 2 weitere Microservice Produktkataloge: Erstellen Sie ein Microservice, welches eine Liste von Produkten anbietet. Der Inhalt der Liste soll dabei aus einem „microservice local datastore" kommen – (Decentralized Data Management). Ersetzen Sie die hard codierten Werte im MeiShop/ProductList-Controller durch den Aufruf des soeben erstellen Services. Ein weiterer Produktkatalog-Service soll Produkte aus einem Text File auf einem FTP-Server auslesen oder einem anderen geeigneten Persistencestore und zur Verfügung stellen. (10 Punkte)

---

## Aufgabe 3 (10 Punkte)

- [x] Skalierung, Ausfallssicherheit und Logging (Design for failure) für CreditPaymentService. Detailsbeschreibung: Publizieren Sie das Service „IEGEasyCreditCardService" mehrfach und rufen Sie die Services im „Round Robin" Stil auf. Falls es beim Aufruf eines Service zu einem Fehler kommt, soll es eine Retry-Logik geben, außerdem soll der aufgetretene Fehler mit Hilfe eines zentralen Logging-Service (gRPC) protokolliert werden. Nach n erfolglosen Versuchen, soll das nächste Service aufgerufen werden. Recherchieren Sie zusätzlich nach einem geeigneten Framework und Skalierungsmöglichkeiten setzen Sie dieses gegebenenfalls ein (10 Punkte)

---

## Aufgabe 4 (10 Punkte)

- [x] (theoretische) Überlegungen zum Einsatz von Asynchronen Kommunikationsstilen in der Handelsplattform (10 Punkte) – Pattern: Messaging

---

## Aufgabe 5 (10 Punkte)

- [x] Schreiben Sie ein zusätzliches „Paymentservice". Dieses Payment-Service soll sowohl JSON, XML-Nachrichten als auch Nachrichten im Format CSV verarbeiten und erzeugen können. Orientieren Sie sich an dem Pattern - HTTP Content Negotiation in REST APIs (restfulapi.net) (10 Punkte)

---

## Aufgabe 6 (10 Punkte)

- [ ] (theoretische) Überlegungen zu einem PaymentService-Broker. Dieses Service soll zwischen Shops und Payment-Services „vermitteln". Mögliche Info-Quellen: Broker Pattern - GeeksforGeeks, Message Broker Pattern for Microservices Interservice Communication | Redis, http://www.enterpriseintegrationpatterns.com/patterns/messaging/CanonicalDataModel.html – Recherchieren Sie dazu zusätzliche Patterns und Quellen

---

## Aufgabe 7 (10 Punkte)

- [ ] Webhook-Subscriber: Überlegen und implementieren Sie ein mögliches Webhook-Szenario (10 Punkte)

---

## Aufgabe 8 (10 Punkte)

- [ ] Machen Sie sich mit dem Begriff OData vertraut. Überlegen und implementieren Sie ein mögliches OData (Service & Client)-Szenario (10 Punkte)

---

## Aufgabe 9 (10 Punkte)

- [ ] Machen Sie sich mit dem Begriff SAGA-Pattern vertraut. Überlegen und implementieren Sie ein mögliches SAGA-Pattern Szenario (Service & Client)-Szenario. Umgang mit Ausfallsicherheit – Stichwort: Design for failure / Resilient Software Design (10 Punkte)

---

## Aufgabe 10 (10 Punkte)

- [ ] Machen Sie sich mit dem Begriff „Open Data" vertraut und beschreiben Sie diesen in einigen wenigen Sätzen. Beschreiben Sie außerdem mögliche Anwendungsfälle im Zusammenhang mit der Handelsplattform

---

---

# TED Aufgaben

---

## TED 1 - Fachartikelanalyse (8 Punkte)

- [ ] Wählen Sie einen der drei bereitgestellten Fachartikel aus. Analysieren Sie die zentrale Argumentation des Beitrags und erläutern Sie, welchen Beitrag er zur Konzeption, Gestaltung oder Weiterentwicklung von Microservice-Architekturen im Kontext elektronischer Geschäftsprozesse leistet. Übertragen Sie die wesentlichen Erkenntnisse auf Ihre Projektarbeit und reflektieren Sie deren Nutzen, Grenzen und Implikationen für Ihre Lösung.

  Artikel zur Auswahl:
  - Ataei & Staegemann (2023) – Application of microservices patterns to big data systems
  - Montesi & Weber (2016) – Circuit Breakers, Discovery, and API Gateways in Microservices
  - Schwarz et al. (2025) - Balancing technology heterogeneity in microservice architectures
  - Xiang et al. (2021) – Microservice Practices Reconsidered in Industry
  - Vogel et al. (2021) – Resilience in the Cyberworld

  Anstelle eines der bereitgestellten Artikel kann auch ein anderer wissenschaftlicher Fachartikel gewählt werden, sofern dessen Auswahl fachlich begründet wird und ein klarer thematischer Bezug zur Projektarbeit besteht.

---

## TED 2 - Makro- und Mikro-Architektur (8 Punkte)

- [ ] Beschreiben Sie die Makro- und Mikro-Architektur Ihrer Lösung zum Thema „most wanTED". Stellen Sie auf Makro-Ebene die zentralen fachlichen Bausteine, die daraus abgeleiteten Microservices sowie deren Kommunikationsbeziehungen dar. Erläutern Sie auf Mikro-Ebene beispielhaft den inneren Aufbau von ein bis zwei Microservices, insbesondere im Hinblick auf Schnittstellen, Geschäftslogik und Datenhaltung. Begründen Sie kurz, inwiefern Ihre Architektur die Anforderungen an Erweiterbarkeit, Austauschbarkeit und Skalierbarkeit unterstützt.

---

## TED 3 - Domain Driven Design (8 Punkte)

- [ ] Nutzen Sie die in Aufgabe 2 entwickelte Makro- und Mikro-Architektur als Ausgangspunkt und konkretisieren Sie den fachlichen und technischen Zuschnitt Ihrer Microservices mit Hilfe von Domain Driven Design. Arbeiten Sie geeignete Bounded Contexts heraus und erläutern Sie die Beziehungen zwischen diesen. Gehen Sie dabei auf geeignete DDD-Konzepte wie Shared Kernel, Customer/Supplier, Conformist, Anticorruption Layer, Separate Ways, Open Host Service und Published Language ein und begründen Sie deren Einsatz oder bewusste Nicht-Verwendung in Ihrem Entwurf. Das Ergebnis dieser Aufgabe soll eine strukturierte Darstellung der Aufgaben der einzelnen Microservices, ihrer Kommunikationswege und -stile, der ausgetauschten Daten einschließlich Formate sowie der Datenhaltung sein (4 Punkte)

- [ ] Beschreiben Sie kurz (max. eine halbe Seite) zwei alternative Ansätze zu Domain Driven Design (4 Punkte)

---

## TED 4 - Implementierung I: Discovery & Configuration (12 Punkte)

- [ ] Implementieren Sie ein Microservice, das zur Unterstützung von Skalierbarkeit und Ausfallsicherheit in mehreren Instanzen deployt und betrieben werden kann. Implementieren Sie zusätzlich ein weiteres Microservice, das dieses Service konsumiert. Beschreiben und demonstrieren Sie, wie das konsumierende Microservice die verfügbaren Instanzen sowie deren Endpunkte dynamisch ermitteln kann. Verwenden Sie hierzu eine geeignete Service-Discovery-Lösung, beispielsweise HashiCorp Consul oder ein vergleichbares Produkt. Realisieren Sie außerdem einen zentralen Konfigurationsdienst, der von mehreren Microservices zur Verwaltung und Bereitstellung von Konfigurationsdaten genutzt wird. Setzen Sie dabei mindestens einmal das Sidecar-Pattern ein und erläutern Sie dessen konkrete Funktion in Ihrem Lösungsentwurf. https://docs.microsoft.com/en-us/azure/architecture/patterns/sidecar

---

## TED 5 - Implementierung II: Secrets (12 Punkte)

- [ ] Implementieren Sie ein Microservice, das für die Kommunikation mit anderen Microservices Secrets oder Tokens benötigt. Beschreiben Sie, wie Secrets und Zugriffstoken in einer Microservice-Umgebung sicher verwaltet und bereitgestellt werden können. Verwenden Sie hierzu eine geeignete Lösung, beispielsweise HashiCorp Vault, Azure Key Vault oder ein vergleichbares Produkt. Implementieren oder verwenden Sie außerdem einen Dienst zur Verwaltung von Secrets, der von dem genannten Microservice genutzt wird. Erläutern Sie in diesem Zusammenhang die Begriffe API Key, SAML, OAuth und OpenID Connect und ordnen Sie diese in den Kontext von Authentifizierung, Autorisierung und sicherer Service-Kommunikation ein. Setzen Sie dabei mindestens einmal das Sidecar-Pattern ein und erläutern Sie dessen Rolle in Ihrer Architektur. https://docs.microsoft.com/en-us/azure/architecture/patterns/sidecar

---

## TED 6 - Implementierung III: Asynchrones Messaging & Content Negotiation (12 Punkte)

- [ ] Implementieren Sie ein Microservice, das asynchrone bzw. ereignisgetriebene Kommunikation mit anderen Diensten nutzt. Beschreiben Sie die Möglichkeiten sowie die Vor- und Nachteile solcher Kommunikationsformen im Kontext von Microservice-Architekturen. Setzen Sie dabei eine der folgenden Varianten konkret ein: Queue (Point-to-Point), Topic (Publish/Subscribe) oder WebHooks. Das Microservice soll außerdem mit unterschiedlichen Datenformaten umgehen können. Verwenden Sie hierfür Content Negotiation und erläutern Sie deren Rolle für die flexible Kommunikation und Integration in einer Microservice-Architektur.

---

## TED 7 - Qualität & Monitoring (12 Punkte)

- [ ] Entwickeln Sie für das Projekt „most wanTED" ein Konzept zur Qualitätssicherung, Überwachung und Analyse Ihrer Microservice-Architektur. Gehen Sie dabei insbesondere auf Integrationstests sowie Last- und Performanztests ein und erläutern Sie deren Bedeutung für die Stabilität und Weiterentwicklung Ihrer Lösung. Beschreiben Sie außerdem geeignete System-, Application- und Business-Metriken im Kontext von „most wanTED" und begründen Sie deren Nutzen für den Betrieb und die Beurteilung der Lösung. Entwickeln Sie darüber hinaus einen zentralen Logging- und Tracing-Ansatz, der ein dienstübergreifendes End-to-End-Tracing von Aufrufen ermöglicht. Erläutern Sie in diesem Zusammenhang den Begriff Observability und beschreiben Sie, wie Logs, Metriken und Traces zusammenwirken, um Verhalten, Zustand und Probleme des Systems nachvollziehbar zu machen. Zeigen Sie abschließend konkrete Maßnahmen auf, mit denen sich die Observability Ihrer Lösung verbessern lässt.

---

## TED 8 - Implementierung: Alternative „Produktempfehlung" (12 Punkte)

- [ ] Ergänzend zur umfragebasierten Entscheidungsunterstützung soll im Projekt „most wanTED" auch die Möglichkeit vorgesehen werden, eine besonders prominente Produktplatzierung kostenpflichtig zu beantragen. Hierzu wird die Spezifikation des zu bewerbenden Produkts an einen SOAP-Endpunkt übermittelt. Die eingehende Promotionsanfrage durchläuft anschließend einen festgelegten Genehmigungsprozess mit mehreren fachlichen Entscheidungsschritten. Am Ende des Prozesses wird die Produktpromotion in Abhängigkeit von Produktpreis, Produktbeschreibung und der Freigabe durch die Gesellschafterinnen und Gesellschafter genehmigt oder abgelehnt. Beschreiben Sie, wie dieser Prozess mit Hilfe einer Workflow-Engine modelliert, ausgeführt und überwacht werden kann. Gehen Sie dabei insbesondere auf Prozessschritte, Zustandsübergänge, beteiligte Rollen sowie die Integration in Ihre bestehende Microservice-Architektur ein. Erläutern Sie außerdem die Begriffe Business Process Modelling und BPEL und ordnen Sie diese fachlich in den beschriebenen Anwendungsfall ein. Vergleichen Sie den BPEL-Ansatz mit modernen Formen der Orchestrierung und Choreographie von Microservices und reflektieren Sie deren jeweilige Eignung für die „most wanTED"-Lösung.

---

## TED 9 - KI, Low-Code & Visionäre Weiterentwicklung (12 Punkte)

- [ ] **a)** KI-gestützte Unterstützung eines Geschäftsprozesses: Beschreiben Sie, wie ein ausgewählter Geschäftsprozess Ihrer Lösung durch den Einsatz von Künstlicher Intelligenz unterstützt, erweitert oder teilweise automatisiert werden kann. Begründen Sie Ihre Auswahl und diskutieren Sie die Vorteile, Grenzen und Risiken des gewählten Ansatzes. (3 Punkte)

- [ ] **b)** Low-Code- oder No-Code-basierte Umsetzung: Erläutern Sie, inwiefern sich derselbe oder ein vergleichbarer Geschäftsprozess mit Hilfe von Low-Code- oder No-Code-Werkzeugen modellieren, integrieren oder umsetzen lässt. Diskutieren Sie auch hier die Vor- und Nachteile und begründen Sie Ihre gewählte Vorgehensweise. (3 Punkte)

- [ ] **c)** Visionäre Weiterentwicklung der Architektur: Diskutieren Sie, welche AI-gestützten Funktionen im Zusammenhang mit Discovery & Configuration, Logging, Messaging, Secrets Management, Observability oder resilienter Softwareentwicklung in Zukunft denkbar oder wünschenswert wären. Wählen Sie zwei der genannten Themenbereiche aus und beschreiben Sie diese ausführlicher. Dabei sind ausdrücklich auch visionäre, experimentelle oder „utopische" Überlegungen erwünscht, sofern diese fachlich nachvollziehbar begründet werden. Nutzen Sie zur Einordnung gerne auch wissenschaftliche Literatur oder geeignete Quellen aus dem FH Campus 02 Bibliothekskatalog. (3 Punkte)

- [ ] **Ergänzend:** Beschreiben Sie im Zusammenhang mit Ihrer Ausarbeitung überblicksartig die Begriffe Agentic AI, RPA und ein aktuelles agentisches Framework wie beispielsweise OpenClaw und ordnen Sie diese in den Kontext moderner Prozessunterstützung ein (max. eine Seite innerhalb der Gesamtbearbeitung). Kurs: Agentic AI: The New Software Paradigm | KI-Campus (3 Punkte)

---

## TED 10 - Aufbereitung und Präsentation (20 Punkte)

- [ ] Die Präsentation dient nicht nur der Darstellung, sondern auch der fachlichen Einordnung und der nachvollziehbaren Demonstration der erarbeiteten Lösung. Ebenso werden eine verständliche, durchdachte und schlüssige schriftliche Dokumentation sowie eine kompakte und sachliche Aufbereitung der Ergebnisse als wesentliche Bestandteile der Projektarbeit betrachtet. Entscheidend sind nicht Umfang, Mindestseitenzahlen oder die bloße Aneinanderreihung bzw. Übernahme von Inhalten, sondern die fachliche Qualität, eigenständige Strukturierung, Nachvollziehbarkeit und Konsistenz der Ausarbeitung. Wird keine Präsentation durchgeführt, können die entsprechenden Punkte für Präsentation, Demonstration und verteidigende Erläuterung nicht erreicht werden.

---

## TED 11 - Funktionierende Gesamtlösung (8 Punkte)

- [ ] Führen Sie die im Rahmen der Projektarbeit entwickelten Komponenten zu einer nachvollziehbaren und demonstrierbaren Gesamtlösung zusammen. Zeigen Sie, dass die wesentlichen Bausteine Ihrer Architektur im Zusammenspiel funktionsfähig sind und die konzipierten Integrationsbeziehungen, Kommunikationswege und technischen Entscheidungen in einer konsistenten Gesamtlösung umgesetzt wurden.

---

---

# BONUS-Punkte (max. 5 Punkte - eines auswählen)

- [ ] **A)** Einsatz des Saga-Patterns: Implementieren Sie konkret 1 Microservice welches das Saga Pattern verwendet (als Ersatz für Distributed Transactions). Beschreiben Sie in diesem Zusammenhang auch das Protokoll 2PC – two-phase commit. https://microservices.io/patterns/data/saga.html (5 Punkte)

- [ ] **B)** Konsumieren eines beliebigen Service aus der „Cloud": Implementieren Sie konkret 1 Microservice welches ein beliebiges „fremdes" (Cloud)-Service verwendet (5 Punkte)

- [ ] **C)** Einsatz des CQRS Patterns: Implementieren Sie konkret 1 Microservice welches das CQRS Pattern verwendet (5 Punkte)

- [ ] **D)** Einsatz von Event Sourcing: Implementieren Sie konkret das Pattern Event Sourcing (5 Punkte)

- [ ] **E)** Überlegen Sie sich ein geeignetes Deployment-Szenario: Implementieren Sie konkret ein geeignetes Deployment-Szenario (5 Punkte)
