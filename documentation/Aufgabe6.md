# Aufgabe 6 - PaymentService-Broker (10 Punkte)

> [!IMPORTANT]
> **Constraint:** Above all, after completing this assignment, it is important to verify that the software still works.

## Aufgabenstellung

(theoretische) Überlegungen zu einem PaymentService-Broker. Dieses Service soll zwischen Shops und Payment-Services „vermitteln".

Mögliche Info-Quellen:
- Broker Pattern - GeeksforGeeks
- Message Broker Pattern for Microservices Interservice Communication | Redis
- http://www.enterpriseintegrationpatterns.com/patterns/messaging/CanonicalDataModel.html

Recherchieren Sie dazu zusätzliche Patterns und Quellen

## Ausarbeitung

Ein **PaymentService-Broker** agiert als zentraler Vermittler (Middleman) zwischen verschiedenen Shops (z.B. MeiShop) und den eigentlichen Payment-Services (z.B. Kreditkarte, PayPal). Anstatt dass jeder Shop jedes Payment-Service direkt kennen und aufrufen muss, kommunizieren alle über den Broker.

### 1. Broker Pattern
Das **Broker Pattern** ist ein Architekturmuster für verteilte Systeme, das Komponenten stark entkoppelt. 
- **Zentrale Instanz:** Der Broker nimmt Anfragen (Messages) von Clients (Shops) entgegen und leitet sie an die zuständigen Server (Payment-Services) weiter.
- **Vorteile:** Lose Kopplung, leichte Skalierbarkeit und Interoperabilität. Die Shops müssen weder die physische Adresse (IP) noch die genaue API-Implementierung der Payment-Services kennen.

### 2. Message Broker Pattern für Microservices (am Beispiel Redis)
Beim Einsatz eines Message Brokers kommunizieren Microservices asynchron über Nachrichten-Warteschlangen (Queues) oder Pub/Sub-Kanäle. **Redis** bietet hierfür zwei primäre Mechanismen:
- **Redis Pub/Sub:** "Fire-and-Forget"-Modell. Eignet sich für Echtzeit-Benachrichtigungen, bietet aber keine Persistenz (Nachrichten gehen verloren, wenn der Empfänger offline ist).
- **Redis Streams:** Ein persistentes Event-Log. Ermöglicht Consumer Groups, Acknowledgements (XACK) und garantierte Auslieferung (At-least-once). Für einen PaymentService-Broker ist **Redis Streams** zwingend zu bevorzugen, da finanzielle Transaktionen zuverlässig und ausfallsicher verarbeitet werden müssen und Nachrichten nicht verloren gehen dürfen.

### 3. Canonical Data Model
Wenn verschiedene Shops und Payment-Services unterschiedliche Datenformate nutzen (z.B. JSON, XML, proprietäre Formate), steigt die Anzahl der benötigten Übersetzer (Message Translators) bei direkter Kommunikation exponentiell an (jeder muss mit jedem sprechen können).
- **Lösung:** Einführung eines **Canonical Data Model** (kanonisches Datenmodell). Das ist ein standardisiertes, anwendungsunabhängiges Zwischenformat, auf das sich alle Teilnehmer im Messaging-System einigen.
- **Vorteil:** Jeder Shop übersetzt sein eigenes Format einmal in das kanonische Format des Brokers. Der Broker leitet die Nachricht weiter, und das Payment-Service übersetzt sie vom kanonischen Format in sein eigenes Zielformat. Dies minimiert Abhängigkeiten und reduziert die Anzahl der benötigten Übersetzungs-Schnittstellen drastisch.
