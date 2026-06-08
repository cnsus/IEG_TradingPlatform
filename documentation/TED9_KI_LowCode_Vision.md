# TED 9 - KI, Low-Code & Visionäre Weiterentwicklung (12 Punkte)

# TED 9 - KI, Low-Code & Visionäre Weiterentwicklung (12 Punkte)

## Aufgabenstellung

(siehe oben)

## Ausarbeitung

### a) KI-gestützte Unterstützung eines Geschäftsprozesses

**Prozess:** Der in TED 8 beschriebene Freigabeprozess für "Prominente Produktplatzierungen".
Anstatt dass Gesellschafter jede Promotionsanfrage manuell prüfen müssen, kann eine **Generative KI (LLM)** als Assistenzsystem integriert werden. Die KI analysiert den Text der eingereichten Produktspezifikation und gleicht diese mit den Richtlinien der Plattform ab (z. B. Filterung von unangemessenen Inhalten, Scam, irreführenden Produktbeschreibungen).
- **Vorteile:** Massive Beschleunigung des Workflows. Die KI kann 90% der offensichtlich korrekten oder inkorrekten Anfragen sofort klassifizieren. Der "Human-in-the-loop" muss nur noch bei unklaren Grenzfällen (Confidence Score < 80%) eingreifen.
- **Grenzen & Risiken:** Gefahr von "Halluzinationen" (falsch-positive Freigaben). Es besteht zudem das Risiko von Bias, falls das Modell bestimmte Sprachstile ungerechtfertigt abwertet. Aus Compliance-Gründen muss jede KI-Entscheidung geloggt und revisionssicher nachvollziehbar sein.

### b) Low-Code- oder No-Code-basierte Umsetzung

Der gesamte Genehmigungsworkflow aus TED 8 lässt sich exzellent mit **Low-Code/No-Code-Plattformen** (z.B. Microsoft Power Automate, n8n oder Zapier) umsetzen, anstatt eine eigene Workflow-Engine in C# zu programmieren.
Die Plattform fungiert als Orchestrator: Sie stellt einen HTTP/SOAP-Trigger bereit, nutzt visuelle Drag-and-Drop Blöcke für die Logik (If/Else-Bedingungen für den Preis) und ruft über standardisierte REST-Konnektoren unseren `PaymentService` auf. Die manuelle Freigabe erfolgt über einen generierten Button in einer Teams-Nachricht oder einer automatisierten E-Mail.
- **Vorteile:** Extreme "Time-to-Market". Der Prozess kann von Fachanwendern (Citizen Developers) modelliert und angepasst werden, ohne die IT-Ressourcen zu belasten.
- **Nachteile:** Vendor Lock-in (Abhängigkeit vom Low-Code-Anbieter), eingeschränkte Versionierbarkeit in Git und potenzielle Sicherheitsrisiken (Schatten-IT), da externe Tools Zugriff auf interne REST-Schnittstellen (wie das Payment) benötigen.

### c) Visionäre Weiterentwicklung der Architektur

Mit Blick in die Zukunft können KI-gestützte Funktionen die Architektur unserer Microservice-Lösung drastisch verändern.

1. **KI-gestütztes Service Discovery & Auto-Scaling:** Aktuell registrieren sich Services statisch bei Consul. In einer utopischen, autonomen Cloud-Umgebung könnte ein KI-Agent die Metriken und den eingehenden Traffic (z.B. am Black Friday) prädiktiv analysieren. Bevor die Lastspitze das Gateway erreicht, instruiert die KI das Container-Orchestrierungssystem (Kubernetes), proaktiv neue Instanzen des `IEGEasyCreditcardService` zu starten. 
2. **AI-driven Observability & Self-Healing:** Das gRPC-Logging sammelt derzeit nur stumpf Fehlertexte. Ein integriertes KI-Modell könnte den Log-Stream in Echtzeit scannen, Anomalien erkennen (z.B. untypisch viele HTTP 500 Fehler im Payment) und *Self-Healing* Maßnahmen ergreifen – wie etwa den fehlerhaften Knoten aus dem Load-Balancer-Pool des API-Gateways zu entfernen und die Saga-Kompensation zu triggern, noch bevor menschliches Eingreifen erforderlich wird.

### Ergänzend: Agentic AI, RPA & agentisches Framework

Die klassische Automatisierung basiert stark auf strikten Regeln. Hier setzt **RPA (Robotic Process Automation)** an: Software-Bots imitieren menschliche Eingaben auf Benutzeroberflächen (Screen Scraping) und führen repetitive, regelbasierte Klick-Strecken aus (z.B. "Daten aus Excel kopieren und in altes ERP einfügen"). RPA ist "dumm" und bricht bei der kleinsten Änderung der UI sofort ab.

Im extremen Kontrast dazu steht **Agentic AI** (Agentische Künstliche Intelligenz). Dies markiert ein neues Software-Paradigma. Ein autonomer Agent führt nicht stur ein Skript aus, sondern bekommt ein übergeordnetes Ziel (z.B. "Überprüfe das Produkt auf Regelkonformität"). Der Agent plant selbstständig die notwendigen Schritte, wählt passende Tools (Web-Suche, API-Calls) und kann bei Fehlern seine Strategie adaptieren. 

Moderne **agentische Frameworks** (wie AutoGPT, LangChain oder das im Kurs erwähnte *OpenClaw*) bieten die Infrastruktur, um solche Multi-Agenten-Systeme zu orchestrieren. Sie statten die LLMs mit Gedächtnis (Memory), Planungsfähigkeiten und Tool-Access aus. 
**Einordnung:** Während klassische Workflow-Engines (BPEL) oder RPA den genauen *Pfad* vorgeben, geben agentische Frameworks nur noch das *Ziel* vor. In der "most wanTED" Plattform könnte ein Agent autonom entscheiden, welche Microservices er ansprechen muss, um das Problem eines unzufriedenen Kundenkulanz-Falls im Shop zu lösen, anstatt diesen in statische Code-Regeln gießen zu müssen.
