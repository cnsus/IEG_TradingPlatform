# Präsentations-Slides: IEG Trading Platform ("most wanTED")

Hier ist die Struktur für eine kurze, knackige PowerPoint-Präsentation (ca. 5-7 Minuten), die sich perfekt als Einstieg vor eurer Live-Demo eignet. Ihr könnt diese Texte einfach in euer FH Campus 02 PowerPoint-Template kopieren.

---

## Slide 1: Titel-Folie
**Titel:** IEG Trading Platform – "most wanTED"
**Untertitel:** Microservice-basierte Handelsplattform
**Referenten:** Hans Erik Krenn, Patrick Grüner, Kevin Ulm
**Datum:** [Datum eintragen]

---

## Slide 2: Projektvision & Zielsetzung
**Titel:** Worum geht es?
**Bulletpoints:**
- **Ausgangslage:** Transformation eines monolithischen Ansatzes in eine moderne Microservice-Architektur.
- **Zielsetzung:** Aufbau einer ausfallsicheren, skalierbaren und entkoppelten E-Commerce-Lösung.
- **Domänen:** Verteilte Verwaltung von Produkten (Catalog), Zahlungen (Payment) und Bestellungen (Orders).
- **Fokus:** Einsatz modernster Integrationstechnologien (Domain-Driven Design, Resilience, Asynchronität).

**🗣️ Speaker Notes (für euch):**
> "Herzlich willkommen zu unserer Projektvorstellung. Wir haben die 'most wanTED' Trading Platform entwickelt. Unser Ziel war es nicht nur Code zu schreiben, sondern eine robuste, Cloud-native Architektur aufzubauen, bei der Ausfallsicherheit und Skalierbarkeit im Vordergrund stehen."

---

## Slide 3: Die Makro-Architektur (Big Picture)
**Titel:** Unsere Architektur im Überblick
**Bulletpoints:**
- **MeiShop (API Gateway):** Der zentrale Einstiegspunkt (BFF) für Clients.
- **Service Discovery:** Dynamisches Routing über HashiCorp Consul.
- **Backend-Services (10 Microservices):** Strikte Trennung nach Bounded Contexts (Product, Payment, Order).
- **Querschnittsfunktionen:** Zentrales High-Performance Logging via gRPC.

*(💡 Tipp für die Folie: Fügt hier das Architektur-Diagramm aus eurer Dokumentation oder dem README als großes Bild ein!)*

**🗣️ Speaker Notes (für euch):**
> "Unsere Architektur besteht aus 11 Services. Das Herzstück nach außen ist das MeiShop API-Gateway. Anstatt IPs hart zu verdrahten, nutzen wir Consul für dynamisches Service Discovery. Unsere Backend-Services sind fachlich streng getrennt und kommunizieren asynchron oder über REST und gRPC."

---

## Slide 4: Technische Highlights & Patterns
**Titel:** Technische Highlights
**Bulletpoints:**
- **SAGA-Pattern (Orchestrierung):** Verteilte Transaktionen für Bestellungen (Kompensation statt Rollbacks).
- **Webhooks & Event-Driven:** Asynchrone Benachrichtigungen bei erfolgreichen Zahlungen.
- **Content Negotiation:** Payment-Service unterstützt dynamisch JSON, XML und CSV.
- **Resilienz (Polly):** Circuit Breaker und Retries fangen Backend-Ausfälle ab.

**🗣️ Speaker Notes (für euch):**
> "Besonders stolz sind wir auf die Implementierung von verteilten System-Mustern. Wir nutzen das Saga-Pattern, um zu garantieren, dass eine Bestellung nicht in einem undefinierten Zustand hängenbleibt, falls das Payment ausfällt. Zudem haben wir asynchrone Webhooks integriert, um Services noch stärker voneinander zu entkoppeln."

---

## Slide 5: Überleitung zur Live-Demo
**Titel:** Live-Demonstration
**Bulletpoints:**
- **Showcase 1:** Service Discovery & Load-Balancing in Aktion.
- **Showcase 2:** Komplexe Produktsuche via OData.
- **Showcase 3:** Ein kompletter Saga-Bestellprozess (Order $\rightarrow$ Payment $\rightarrow$ Webhook).

**🗣️ Speaker Notes (für euch):**
> "Genug der Theorie. Wir möchten euch nun live zeigen, wie das System im Zusammenspiel funktioniert. Wir starten mit Consul und arbeiten uns bis zu einer vollständig orchestrierten Bestellung durch..."
*(Hier wechselt ihr in euren Browser/Postman und folgt dem Demo-Skript aus TED 10).*

---

### Tipps für die Präsentation:
- **Weniger ist mehr:** Haltet die Folien minimalistisch. Die Dozenten wollen sehen, dass ihr die Konzepte verstanden habt und flüssig demonstrieren könnt.
- **Fokus auf das Demo:** Verbringt maximal 30% der Zeit mit den Slides und 70% mit der Live-Demo.
