# TED 11 - Funktionierende Gesamtlösung (8 Punkte)

# TED 11 - Funktionierende Gesamtlösung (8 Punkte)

## Aufgabenstellung

Führen Sie die im Rahmen der Projektarbeit entwickelten Komponenten zu einer nachvollziehbaren und demonstrierbaren Gesamtlösung zusammen. Zeigen Sie, dass die wesentlichen Bausteine Ihrer Architektur im Zusammenspiel funktionsfähig sind und die konzipierten Integrationsbeziehungen, Kommunikationswege und technischen Entscheidungen in einer konsistenten Gesamtlösung umgesetzt wurden.

## Ausarbeitung

### Zusammenführung und Lauffähigkeit

Die "most wanTED" Handelsplattform wurde im Zuge der Entwicklung vollständig in eine saubere Microservice-Architektur refaktorisiert. Alle 11 Microservices (inkl. des zentralen API-Gateways) liegen nun gebündelt im `src/` Verzeichnis und können über das mitgelieferte Shell-Skript (`start-all.sh` bzw. `start-all.bat`) mit einem einzigen Befehl hochgefahren werden.

Die Gesamtlösung beweist ihre Funktionsfähigkeit durch das nahtlose Zusammenspiel der folgenden Architekturbausteine:
1. **Service Discovery (Consul):** Startet das Skript, registrieren sich sofort alle Services – auch die mehrfach instanziierten `IEGEasyCreditcardService` Knoten – erfolgreich bei HashiCorp Consul.
2. **API Gateway (MeiShop):** Das Frontend nutzt Consul, um den Traffic dynamisch und ausfallsicher (mit Polly Circuit Breakern) an die Backend-Services zu routen.
3. **Orchestrierung (Saga-Pattern):** Wird eine Bestellung getätigt, übernimmt der `OrderSagaService` die Regie. Er validiert zuerst den Lagerbestand beim `ProductService` und ruft dann den `PaymentService` auf. 
4. **Asynchrone Event-Verarbeitung (Webhooks):** Sobald die Zahlung im Saga-Prozess genehmigt wurde, feuert der PaymentService asynchron einen Webhook an den `WebhookSubscriberService`, was demonstriert, dass lose gekoppelte ereignisbasierte Kommunikation einwandfrei funktioniert.
5. **Observability (gRPC Logging):** Jeder dieser Schritte – von der Registrierung bis zur Buchung – wird im Hintergrund nahezu latenzfrei via gRPC an den `LoggingService` gestreamt, was die lückenlose Nachvollziehbarkeit des Gesamtsystems garantiert.

Damit ist nachgewiesen, dass die konzipierten Integrationsbeziehungen in der Praxis konsistent umgesetzt wurden und die Lösung in ihrer Gesamtheit hochverfügbar und funktional ist.
