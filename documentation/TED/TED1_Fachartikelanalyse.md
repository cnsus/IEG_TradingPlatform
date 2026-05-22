# TED 1 - Fachartikelanalyse (8 Punkte)

# TED 1 - Fachartikelanalyse (8 Punkte)

## Aufgabenstellung

Wählen Sie einen der drei bereitgestellten Fachartikel aus. Analysieren Sie die zentrale Argumentation des Beitrags und erläutern Sie, welchen Beitrag er zur Konzeption, Gestaltung oder Weiterentwicklung von Microservice-Architekturen im Kontext elektronischer Geschäftsprozesse leistet. Übertragen Sie die wesentlichen Erkenntnisse auf Ihre Projektarbeit und reflektieren Sie deren Nutzen, Grenzen und Implikationen für Ihre Lösung.

Artikel zur Auswahl:
- Montesi & Weber (2016) – Circuit Breakers, Discovery, and API Gateways in Microservices
- (weitere Artikel ausgeblendet...)

## Ausarbeitung

### 1. Zentrale Argumentation des Artikels
Der Artikel von *Montesi & Weber (2016)* befasst sich mit den fundamentalen Herausforderungen verteilter Systeme, insbesondere der stark gestiegenen Netzwerkkommunikation und der Fehleranfälligkeit in Microservice-Architekturen. Die Kernargumentation besagt, dass die Aufteilung eines Monolithen zwar Agilität und unabhängige Skalierbarkeit fördert, jedoch zwingend spezielle Design-Pattern erfordert, um nicht an Netzwerk-Latenzen oder kaskadierenden Ausfällen zu scheitern. 
Die Autoren stellen drei zentrale Architekturmuster vor:
- **API Gateways:** Dienen als zentraler Einstiegspunkt für externe Clients, bündeln API-Aufrufe und verbergen die interne Komplexität der Systemlandschaft.
- **Service Discovery:** Ermöglicht das dynamische Auffinden von Services zur Laufzeit, da IPs und Ports in Cloud-Umgebungen nicht statisch verlässlich sind.
- **Circuit Breakers:** Ein Resilienz-Muster, das Netzwerkaufrufe überwacht und bei wiederholten Fehlschlägen die Verbindung "kappt", um das System vor Überlastung (kaskadierenden Fehlern) zu schützen und schnelle Fehlermeldungen ("Fail Fast") zu generieren.

### 2. Beitrag zur Konzeption elektronischer Geschäftsprozesse
Im E-Commerce ist Ausfallsicherheit extrem kritisch (z. B. beim Checkout-Prozess). Der Artikel liefert das theoretische Rüstzeug, um elektronische Geschäftsprozesse ausfallsicher zu orchestrieren. Anstatt dass ein Ausfall der Payment-Schnittstelle den gesamten Shop lahmlegt, erlauben Circuit Breakers ein geordnetes Fallback (z. B. "Zahlungsmittel aktuell nicht verfügbar, bitte später probieren"). API-Gateways ermöglichen es zudem, komplexe Prozesse (Bestandsabfrage, Preiskalkulation, Kundendaten) für das Frontend in einem einzigen, effizienten Aufruf zu bündeln, was besonders für mobile Endgeräte unerlässlich ist.

### 3. Übertragung auf "most wanTED"
Die theoretischen Erkenntnisse des Artikels spiegeln sich 1:1 in den Architektur-Entscheidungen der *most wanTED*-Lösung wider:
- **API Gateway:** Unser `MeiShop`-Service fungiert genau als solches Gateway. Es verbirgt die komplexen Backend-Services (Payment, Catalog, Saga) vor dem Browser-Client und orchestriert die Aufrufe.
- **Service Discovery:** Anstelle statischer Konfigurationen nutzen wir **HashiCorp Consul**. Startet ein neuer `IEGEasyCreditcardService`, registriert er sich bei Consul. Das Gateway fragt Consul dynamisch nach den aktuellen IPs ab, was echtes Load-Balancing und Skalierung ermöglicht.
- **Circuit Breakers & Resilience:** Im `MeiShop`-Gateway wurde mit der Bibliothek **Polly** gearbeitet. Ist der Backend-Service nicht erreichbar, greifen definierte Retry- und Circuit-Breaker-Policies, um den Shop responsiv zu halten.

### 4. Reflexion: Nutzen, Grenzen und Implikationen
- **Nutzen:** Die Umsetzung dieser Pattern in unserem Projekt hat die Stabilität massiv erhöht. Durch das Service Discovery können wir nun beliebig viele Instanzen der Kreditkartenprüfung parallel hochfahren, ohne die Konfiguration des Shops ändern zu müssen.
- **Grenzen:** Die Implikation dieser Architektur ist eine deutlich höhere operative Komplexität. Das API-Gateway wird zum **Single Point of Failure (SPOF)** und zu einem potenziellen Performance-Flaschenhals, wenn es nicht selbst redundant ausgelegt ist. Zudem erschwert die dynamische Natur der Service Discovery das Debugging, weshalb wir zwingend einen zentralen `LoggingService` (via gRPC) einführen mussten, um den Zustand des Gesamtsystems überhaupt noch nachvollziehen zu können.
