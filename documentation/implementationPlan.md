# Implementierungsplan – IEG Trading Platform

## Teamaufteilung: Patrick, Hans-Erik, Kevin

> [!NOTE]
> **TED Wahlpflicht:** Als kleinere Gruppe nur **2 von 6** nötig → **TED 8 + TED 9** ✅
>
> **Bonus-Aufgabe Saga:** Wird in TED 11 (Gesamtlösung) direkt integriert.
>
> **Aufgaben 6 + 7:** Nicht nötig → entfernt.

---

## Aufgaben-Übersicht (nur was zu tun ist)

| Aufgabe | Punkte | Typ |
|---------|--------|-----|
| Aufgabe 8 – OData | 10 | Doku+Konzept |
| Aufgabe 9 – SAGA-Pattern | 10 | Doku+Konzept |
| Aufgabe 10 – Open Data | 10 | Doku |
| TED 1 – Fachartikelanalyse | 8 | Doku |
| TED 2 – Makro/Mikro-Architektur | 8 | Doku |
| TED 3 – Domain Driven Design | 8 | Doku |
| TED 8 – Produktempfehlung/SOAP/Workflow | 12 | Doku+Konzept |
| TED 9 – KI, Low-Code, Vision | 12 | Doku |
| TED 10 – Präsentation & Dokumentation | 20 | Gemeinsam |
| TED 11 – Funktionierende Gesamtlösung | 8 | Impl+Doku |
| Bonus A – Saga-Pattern Implementierung | 5 | Impl |
| **Gesamt** | **111** | **~37P pro Person** |

---

## 👤 Patrick (~37 Punkte)

| Aufgabe | Punkte | Beschreibung |
|---------|--------|--------------|
| **Aufgabe 8** | 10 | OData – Theorie + Szenario für Trading Platform |
| **TED 1** | 8 | Fachartikelanalyse |
| **TED 8** | 12 | Produktempfehlung, SOAP, Workflow-Engine, BPEL |
| **TED 10** (Anteil) | ~7 | Präsentation – Slides erstellen |
| | **~37P** | |

### Details

- **Aufgabe 8:** OData-Grundlagen, Szenario z.B. OData-Endpunkt für Produktabfragen ($filter, $orderby). Projektbezug most wanTED: Fragebogen-Ergebnisse per OData abfragen.
- **TED 1:** Artikel analysieren, zentrale Argumentation, Übertragung auf Projektarbeit, Reflexion Nutzen/Grenzen.
- **TED 8:** SOAP-Endpunkt für Promotionsanfrage, Genehmigungsprozess mit Workflow-Engine (z.B. Elsa Workflows), BPMN-Diagramm, Begriffe BPM & BPEL, Vergleich BPEL vs. Orchestrierung/Choreographie.

---

## 👤 Hans-Erik (~37 Punkte)

| Aufgabe | Punkte | Beschreibung |
|---------|--------|--------------|
| **Aufgabe 10** | 10 | Open Data – Definition & Anwendungsfälle |
| **TED 2** | 8 | Makro- und Mikro-Architektur für "most wanTED" |
| **TED 9** | 12 | KI, Low-Code, Vision (alle 4 Teile: a/b/c/ergänzend) |
| **TED 10** (Anteil) | ~7 | Dokumentation konsolidieren |
| | **~37P** | |

### Details

- **Aufgabe 10:** Open Data Definition, OGD-Prinzipien, Anwendungsfälle (Wechselkurse, Produktstandards, etc.).
- **TED 2:** Makro-Ebene (alle Services, Kommunikationsdiagramm), Mikro-Ebene (1–2 Services im Detail), Begründung Erweiterbarkeit/Skalierbarkeit.
- **TED 9:** a) KI-Unterstützung Geschäftsprozess (3P), b) Low-Code/No-Code (3P), c) Visionäre Weiterentwicklung (3P), Ergänzend: Agentic AI, RPA, OpenClaw (3P).

---

## 👤 Kevin (~37 Punkte)

| Aufgabe | Punkte | Beschreibung |
|---------|--------|--------------|
| **Aufgabe 9** | 10 | SAGA-Pattern – Theorie & Szenario |
| **TED 3** | 8 | Domain Driven Design + 2 Alternativen |
| **TED 11** | 8 | Funktionierende Gesamtlösung |
| **Bonus A** | 5 | Saga-Pattern in Microservice implementieren |
| **TED 10** (Anteil) | ~6 | Demo-Skript, start-all.bat aktualisieren |
| | **~37P** | |

### Details

- **Aufgabe 9:** SAGA-Pattern Grundlagen, Choreography vs. Orchestration, Compensating Transactions, Design for Failure.
- **TED 3:** Bounded Contexts für most wanTED, DDD-Konzepte (Shared Kernel, ACL, etc.), Context Map, 2 alternative Ansätze (½ Seite).
- **TED 11 + Bonus:** Gesamtlösung zusammenführen. Einen Microservice (z.B. OrderService) mit Saga-Pattern implementieren + 2PC-Beschreibung.

---

## Arbeitsreihenfolge

```
Phase 1: Arbeitsblatt 8, 9, 10        (alle drei parallel)
Phase 2: TED 1, 2, 3                  (parallel, TED 2 vor TED 3)
Phase 3: TED 8, 9                     (parallel)
Phase 4: TED 11 + Bonus Saga          (Kevin, mit Input von allen)
Phase 5: TED 10 – Präsentation & Doku (gemeinsam)
```

---

## Offene Fragen

1. **Fachartikel TED 1:** Welchen Artikel? Vorschlag: Montesi & Weber 2016
2. **Sprache:** Deutsch oder Englisch?
3. **Womit soll ich anfangen?**
