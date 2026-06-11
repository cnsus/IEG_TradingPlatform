# Aufgabe 7 - Webhook-Subscriber (10 Punkte)

## Aufgabenstellung

Webhook-Subscriber: Überlegen und implementieren Sie ein mögliches Webhook-Szenario (10 Punkte)

## Ausarbeitung

### Was ist ein Webhook?

Ein **Webhook** (auch „HTTP Callback" oder „Reverse API" genannt) ist ein Mechanismus, bei dem ein Server (der **Publisher**) automatisch einen HTTP-POST-Request an eine zuvor registrierte URL (den **Subscriber**) sendet, sobald ein bestimmtes Ereignis eintritt. Im Gegensatz zum klassischen Polling, bei dem ein Client wiederholt den Server nach neuen Daten abfragt, kehrt ein Webhook das Kommunikationsmuster um: Der Server benachrichtigt den Client proaktiv.

#### Polling vs. Webhook

| Eigenschaft       | Polling                            | Webhook                                |
|-------------------|------------------------------------|----------------------------------------|
| Ausloeser         | Client fragt regelmaessig an       | Server sendet bei Ereignis automatisch |
| Latenz            | Hoch (abhaengig vom Intervall)     | Niedrig (nahezu Echtzeit)              |
| Ressourcenverbrauch | Hoch (viele leere Anfragen)      | Niedrig (nur bei Ereignis)             |
| Komplexitaet      | Einfach zu implementieren          | Erfordert Callback-Endpunkt beim Client|

### Projektbezug: most wanTED

Im Kontext der Handelsplattform „most wanTED" wurde folgendes Webhook-Szenario implementiert:

**Ereignis:** Wenn im `PaymentService` ein neues Payment erstellt wird (POST `/api/payments`), werden alle registrierten Webhook-Subscriber automatisch ueber die neue Zahlung benachrichtigt.

### Implementiertes Szenario

#### 1. PaymentService als Webhook-Publisher (Port 7400)

Der bestehende `PaymentService` wurde um Webhook-Funktionalitaet erweitert:

**Neue Komponenten:**
- **`WebhookSubscription` (Model):** Repraesentiert einen registrierten Subscriber mit Callback-URL und Event-Typ.
- **`IWebhookService` / `WebhookService`:** Verwaltet die Subscriber-Liste und feuert HTTP-POST-Callbacks an alle registrierten URLs, wenn ein neues Payment erstellt wird.
- **`WebhookSubscriptionsController`:** REST-API zur Verwaltung der Webhook-Registrierungen.

**Neue Endpunkte im PaymentService:**

| Methode | Endpunkt                      | Beschreibung                                |
|---------|-------------------------------|---------------------------------------------|
| GET     | `/api/webhooksubscriptions`   | Alle registrierten Subscriber anzeigen      |
| POST    | `/api/webhooksubscriptions`   | Neuen Subscriber registrieren (Callback-URL)|
| DELETE  | `/api/webhooksubscriptions/{id}` | Subscriber entfernen                     |

**Ablauf bei Payment-Erstellung:**
1. Client erstellt ein neues Payment via `POST /api/payments`
2. Das Payment wird im lokalen Datastore gespeichert
3. Der `WebhookService` iteriert ueber alle aktiven Subscriber
4. Fuer jeden Subscriber wird ein HTTP POST mit dem Payment-Payload an die Callback-URL gesendet
5. Fehlerhafte Zustellungen werden geloggt, blockieren aber nicht den Hauptprozess

#### 2. WebhookSubscriberService (Port 7600)

Ein neuer, eigenstaendiger Microservice, der als Webhook-Empfaenger fungiert:

**Architektur:**
```
Controllers/
  WebhookController.cs      → Empfaengt Webhook-Benachrichtigungen
Models/
  PaymentNotification.cs     → Datenmodell fuer empfangene Benachrichtigungen
Services/
  INotificationRepository.cs → Interface (Repository Pattern)
  NotificationRepository.cs  → In-Memory Speicher fuer empfangene Notifications
Program.cs                   → Konfiguration und Service-Registrierung
```

**Endpunkte:**

| Methode | Endpunkt                    | Beschreibung                                    |
|---------|-----------------------------|-------------------------------------------------|
| POST    | `/api/webhook/payment`      | Empfaengt Webhook-Callbacks vom PaymentService   |
| GET     | `/api/webhook/notifications`| Zeigt alle empfangenen Benachrichtigungen an     |

### Testen des Webhook-Szenarios

**Schritt 1:** Beide Services starten (PaymentService :7400, WebhookSubscriberService :7600)

**Schritt 2:** Webhook-Subscriber registrieren:
```bash
curl -X POST https://localhost:7400/api/webhooksubscriptions \
  -H "Content-Type: application/json" \
  -d '{"callbackUrl": "https://localhost:7600/api/webhook/payment", "eventType": "payment.created"}' \
  -k
```

**Schritt 3:** Neues Payment erstellen (loest Webhook aus):
```bash
curl -X POST https://localhost:7400/api/payments \
  -H "Content-Type: application/json" \
  -d '{"amount": 199.99, "currency": "EUR", "description": "Webhook-Test", "paymentMethod": "CreditCard"}' \
  -k
```

**Schritt 4:** Empfangene Benachrichtigungen pruefen:
```bash
curl https://localhost:7600/api/webhook/notifications -k
```

### Verwendete Patterns

- **Observer Pattern (via HTTP):** Der PaymentService agiert als Subject, die Subscriber als Observer.
- **Repository Pattern:** Sowohl im PaymentService (IPaymentRepository) als auch im WebhookSubscriberService (INotificationRepository) wird das Repository Pattern fuer den Datenzugriff verwendet.
- **Dependency Injection:** Alle Services werden ueber den ASP.NET Core DI-Container injiziert.
- **Fire-and-Forget:** Webhook-Zustellungsfehler werden geloggt, blockieren aber nicht die Payment-Erstellung.
