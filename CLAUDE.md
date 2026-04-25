# CLAUDE.md — Anweisungen für KI-Assistenten

> Verbindliche Richtlinie für alle KI-Tools (Claude Code, Copilot, etc.), die an diesem Projekt mitarbeiten.
> Ziel: Konsistente Ergebnisse, egal welches Teammitglied gerade arbeitet.

## Team & Kontext

- **Projekt:** IEG Trading Platform – FH Campus 02, Integrationstechnologien für eGovernment
- **Team:** Hans Erik Krenn, Patrick Grüner, Kevin Ulm
- **Stack:** .NET 8, C#, Microservice-Architektur, Consul (Service Discovery), gRPC (Logging)
- **Zweck:** Studienprojekt — bewertet wird die Erfüllung der Aufgabenstellung, nicht Produktionsreife.

---

## Goldene Regeln (immer befolgen)

### 1. Vor jeder Aufgabe: Prüfen, ob bereits umgesetzt
Bevor irgendein Code geschrieben oder eine Aufgabe begonnen wird:

1. [documentation/0Aufgabenstellung.md](documentation/0Aufgabenstellung.md) öffnen → Status der Checkbox prüfen (`[x]` = erledigt, `[ ]` = offen).
2. Die zugehörige Datei `documentation/AufgabeN.md` lesen → ist im Abschnitt "Ausarbeitung" bereits Inhalt vorhanden?
3. Den relevanten Service-Ordner prüfen (z. B. [PaymentService/](PaymentService/), [ProductService/](ProductService/)) → existiert die Funktionalität schon im Code?
4. Falls ja: **Nicht neu implementieren.** Stattdessen Rückfrage an User: "Aufgabe X scheint bereits umgesetzt in Datei Y. Soll ich erweitern, überarbeiten oder etwas anderes machen?"

### 2. Mini-Doku-Pflicht für jede Aufgabe
Jede bearbeitete Aufgabe **muss** in der entsprechenden Datei unter [documentation/](documentation/) dokumentiert werden:

- **Existierende Datei:** `documentation/AufgabeN.md` (für Aufgabe 1–10) bzw. `documentation/TEDX_*.md` für TED-Themen.
- **Format pro Aufgabe** (siehe [documentation/Aufgabe5.md](documentation/Aufgabe5.md) als Vorlage):
  ```markdown
  ## Aufgabenstellung
  <Original-Text aus 0Aufgabenstellung.md>

  ## Ausarbeitung
  <Kurze Erklärung in 3–8 Sätzen, was umgesetzt wurde und warum.>

  **Technische Umsetzung:**
  - Stichpunkte zu Klassen, Patterns, Frameworks
  - Verwendete NuGet-Pakete (falls relevant)

  **Endpoints / Konfiguration** (falls relevant — als Tabelle)

  ### Dateien
  | Datei | Beschreibung |
  |---|---|
  | `Pfad/zur/Datei.cs` | Was sie macht |
  ```
- **Sprache:** Deutsch (wie der Rest der Doku). Keine Umlaute in Code-Identifikatoren, in Prosa erlaubt.
- **Nach Abschluss:** Checkbox in [documentation/0Aufgabenstellung.md](documentation/0Aufgabenstellung.md) auf `[x]` setzen.

### 3. Code: Einfach halten
- **Nur Grundfunktionalität.** Keine vorausschauende Architektur, keine Abstraktionen "auf Vorrat".
- **Keine Production-Härtung:** kein aufwendiges Error Handling, keine Caching-Layer, keine Authentifizierung — außer die Aufgabe verlangt es explizit.
- **In-Memory-Datenhaltung bevorzugt** (Liste/Dictionary in einem Repository-Klässchen). Nur Datenbank, wenn Aufgabenstellung sie verlangt.
- **Keine Tests schreiben**, außer die Aufgabe verlangt sie ausdrücklich.
- **Keine neuen NuGet-Pakete**, wenn die Standard-Library reicht. Wenn nötig: Polly (Retry), Consul.NET (Discovery), Grpc.Net.Client (Logging) sind bereits etabliert.

### 4. Hardcoding ist erlaubt — aber markieren
Hardcodierte Werte (Beispieldaten, Test-Credentials, Demo-Listen) sind ok. Pflicht:

```csharp
// BEISPIEL-DATEN (Hardcoded für Demo-Zwecke)
var products = new List<Product>
{
    new() { Id = 1, Name = "Laptop", Price = 1200m },
    // ...
};
```

oder bei Konfigurationswerten:
```csharp
// BEISPIEL: feste FTP-URL — in Produktion über appsettings.json konfigurieren
const string FtpHost = "ftp://example.com";
```

Markierung in **Deutsch** mit dem Schlüsselwort **`BEISPIEL`** oder **`HARDCODED`**, damit es bei Code-Reviews und Suche schnell auffindbar ist.

### 5. Konsistenz mit bestehendem Code
- **Bestehende Patterns übernehmen** statt neue einführen. Beispiele:
  - Repository-Pattern wie in [PaymentService/Services/PaymentRepository.cs](PaymentService/Services/PaymentRepository.cs)
  - Controller-Struktur wie in [PaymentService/Controllers/PaymentsController.cs](PaymentService/Controllers/PaymentsController.cs)
  - Port-Konvention: siehe Tabelle in [Readme.md](Readme.md) — **keine** neuen Ports erfinden ohne Rücksprache.
- Vor jeder neuen Datei: existiert ein vergleichbares Schema in einem anderen Service? Dann analog aufbauen.

---

## Service-Übersicht (Stand: aktuell)

| Service | Port | Zweck | Status |
|---|---|---|---|
| MeiShop | 5148 | Frontend / API Gateway | vorhanden |
| ProductService | 5221 | Produkte aus Local Datastore | vorhanden |
| FtpProductCatalogService | 5171 | Produkte aus FTP | vorhanden |
| IEGEasyCreditCardService | 5201/5202 | Kreditkarten, multi-instance | vorhanden |
| PaymentService | 5069 | JSON/XML/CSV Content Negotiation | vorhanden |
| LoggingService | 5223 | gRPC Logging | vorhanden |
| Consul | 8500 | Service Discovery | extern |

> Vor Hinzufügen eines Services: prüfen, ob die Aufgabe nicht in einem bestehenden Service erweitert werden kann.

---

## Workflow für neue Aufgaben

1. **Aufgabenstellung lesen:** [documentation/0Aufgabenstellung.md](documentation/0Aufgabenstellung.md) → relevanten Punkt finden.
2. **Status prüfen** (Goldene Regel 1).
3. **Plan kommunizieren:** kurze Antwort an User mit "Was ich tun werde", bevor Code geschrieben wird.
4. **Implementieren** (Goldene Regel 3 + 4).
5. **Dokumentieren** in `documentation/AufgabeN.md` (Goldene Regel 2).
6. **Build prüfen:** `dotnet build SolTradingPlatform.sln` muss durchlaufen.
7. **Checkbox setzen** in `0Aufgabenstellung.md`.

---

## Was NICHT tun

- ❌ Kein eigenmächtiges Refactoring fertiger Aufgaben.
- ❌ Keine Frameworks/Tools einführen, die nicht in der Aufgabenstellung verlangt sind (kein MediatR, kein AutoMapper, kein FluentValidation).
- ❌ Keine `Program.cs`-Umstrukturierung in bestehenden Services, außer die Aufgabe verlangt es.
- ❌ Keine Doku-Dateien außerhalb von [documentation/](documentation/) anlegen (Ausnahme: [Readme.md](Readme.md), [Kurzdoku.md](Kurzdoku.md), diese Datei).
- ❌ Keine englischen Texte in der Doku — Sprache ist Deutsch.
- ❌ Niemals `git push`, `git commit --amend` oder destruktive Git-Befehle ohne explizite User-Aufforderung.

---

## Schnellstart Build & Run

```bash
# Build aller Services
dotnet build SolTradingPlatform.sln

# Alle Services starten
./start-all.sh        # Linux/macOS
start-all.bat         # Windows

# Einzelnen Service starten
cd <ServiceName> && dotnet run
```

---

## Bei Unklarheiten

**Nachfragen statt raten.** Lieber eine Rückfrage zu viel als ein Implementierungsweg, der zurückgebaut werden muss. Insbesondere bei:

- Aufgaben, die mehrere Auslegungen zulassen (z. B. "asynchrone Kommunikation" — Theorie oder Implementierung?)
- Bonus-Aufgaben aus [documentation/Bonus_Aufgaben.md](documentation/Bonus_Aufgaben.md)
- Wenn die Aufgabenstellung Punkte mit "(theoretische) Überlegungen" enthält → reine Doku, kein Code.
