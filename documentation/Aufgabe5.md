# Aufgabe 5 - Payment-Service mit Content Negotiation (10 Punkte)

## Aufgabenstellung

Schreiben Sie ein zusätzliches „Paymentservice". Dieses Payment-Service soll sowohl JSON, XML-Nachrichten als auch Nachrichten im Format CSV verarbeiten und erzeugen können. Orientieren Sie sich an dem Pattern - HTTP Content Negotiation in REST APIs (restfulapi.net) (10 Punkte)

## Ausarbeitung

Der **PaymentService** (Port 7400) implementiert vollstaendige HTTP Content Negotiation fuer drei Formate: JSON, XML und CSV.

**Technische Umsetzung:**
- Der `PaymentsController` deklariert mit `[Produces]` und `[Consumes]` die unterstuetzten MIME-Types (`application/json`, `application/xml`, `text/csv`)
- ASP.NET Core waehlt automatisch das richtige Format basierend auf `Accept`- und `Content-Type`-Header
- Ein **CsvOutputFormatter** serialisiert `Payment`-Objekte in CSV mit Header-Zeile (Id,Amount,Currency,...)
- Ein **CsvInputFormatter** parst CSV-Zeilen zurueck in `Payment`-Objekte
- `Program.cs` registriert beide Custom-Formatter zusaetzlich zu den Standard-Formattern (`AddXmlSerializerFormatters`) und setzt `RespectBrowserAcceptHeader = true`
- Das `Payment`-Model traegt eine `[XmlRoot]`-Annotation fuer XML-Serialisierung
- Das `PaymentRepository` enthaelt drei Beispiel-Zahlungsdatensaetze (In-Memory)

**Endpoints:**
| Methode | Endpunkt | Beschreibung |
|---|---|---|
| GET | `/api/payments` | Alle Payments |
| GET | `/api/payments/{id}` | Einzelnes Payment |
| POST | `/api/payments` | Neues Payment anlegen |

### Dateien

| Datei | Beschreibung |
|---|---|
| `PaymentService/Controllers/PaymentsController.cs` | REST-Controller mit Content Negotiation |
| `PaymentService/Formatters/CsvOutputFormatter.cs` | CSV-Ausgabe-Formatter |
| `PaymentService/Formatters/CsvInputFormatter.cs` | CSV-Eingabe-Formatter |
| `PaymentService/Models/Payment.cs` | Payment-Model mit XML-Annotation |
| `PaymentService/Services/PaymentRepository.cs` | In-Memory Datenhaltung |
| `PaymentService/Program.cs` | Formatter-Registrierung |
