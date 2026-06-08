# Aufgabe 8 - OData (10 Punkte)

## Aufgabenstellung

Machen Sie sich mit dem Begriff OData vertraut. Überlegen und implementieren Sie ein mögliches OData (Service & Client)-Szenario (10 Punkte)

## Ausarbeitung

### Was ist OData?

**OData (Open Data Protocol)** ist ein offener Standard (OASIS-Standard), der von Microsoft initiiert wurde und eine einheitliche REST-basierte Schnittstelle fuer den Zugriff auf strukturierte Daten definiert. OData baut auf etablierten Web-Technologien wie HTTP, JSON und dem Atom Publishing Protocol auf.

Der zentrale Mehrwert von OData liegt in der standardisierten **Query-Sprache**, die es Clients ermoeglicht, Daten serverseitig zu filtern, sortieren, paginieren und zu selektieren — aehnlich einer SQL-Abfrage, aber ueber HTTP:

| Query-Option | Beschreibung                  | Beispiel                           |
|-------------|-------------------------------|-------------------------------------|
| `$filter`   | Filtert Ergebnisse            | `$filter=Price gt 500`             |
| `$orderby`  | Sortiert Ergebnisse           | `$orderby=Price desc`              |
| `$top`      | Limitiert die Anzahl          | `$top=5`                           |
| `$skip`     | Ueberspringt Eintraege        | `$skip=10`                         |
| `$select`   | Waehlt bestimmte Felder       | `$select=Name,Price`               |
| `$count`    | Gibt die Gesamtanzahl zurueck | `$count=true`                      |
| `$metadata` | Beschreibt das Datenmodell    | `GET /odata/$metadata`             |

### Projektbezug: most wanTED

OData kann im Projekt „most wanTED" dazu verwendet werden, die Produktdaten der Handelsplattform generisch ueber ein SQL-aehnliches Abfrageinterface bereitzustellen. Anstatt fuer jeden Anwendungsfall einen eigenen API-Endpunkt zu erstellen (z.B. „guenstige Produkte", „Laptops sortiert nach Preis"), koennen Clients ueber einen einzigen OData-Endpunkt flexible Abfragen formulieren.

### Implementiertes Szenario

Es wurde ein vollstaendiges **OData Service & Client** Szenario implementiert:

#### 1. OData Service: ProductODataService (Port 7500)

Ein neuer, eigenstaendiger Microservice, der den Produktkatalog der Handelsplattform ueber das OData v4 Protokoll bereitstellt.

**Architektur:**
```
ProductODataService/
├── Controllers/
│   └── ProductsController.cs   ← OData-faehiger Controller (ODataController)
├── Models/
│   └── Product.cs              ← Produkt-Entitaet mit 9 Feldern
├── Services/
│   ├── IProductODataRepository.cs   ← Repository-Interface (IQueryable)
│   └── ProductODataRepository.cs    ← In-Memory Datastore (10 Produkte)
├── Program.cs                  ← OData EDM Model + Service-Registrierung
└── Properties/
    └── launchSettings.json     ← HTTPS: 7500, HTTP: 5501
```

**Technische Details:**
- Verwendet `Microsoft.AspNetCore.OData` NuGet-Paket
- Das **Entity Data Model (EDM)** wird ueber `ODataConventionModelBuilder` aufgebaut
- Der Controller erbt von `ODataController` und verwendet `[EnableQuery]` fuer automatische Query-Auswertung
- Das Repository gibt `IQueryable<Product>` zurueck, damit OData die Abfragen direkt auf der Datenquelle ausfuehren kann
- 10 Beispielprodukte in 6 Kategorien (Laptops, Smartphones, Audio, Wearables, Tablets, Zubehoer, Monitore)

#### 2. OData Client: MeiShop API Gateway

Im MeiShop API Gateway wurde ein neuer `ODataProductsController` implementiert, der als **OData Client** fungiert:

- Endpunkt: `GET /api/ODataProducts`
- Leitet alle OData-Query-Parameter (`$filter`, `$orderby`, etc.) transparent an den ProductODataService weiter
- Behandelt Fehler (Service nicht erreichbar → 503 Service Unavailable)

#### Architekturdiagramm

```
┌──────────┐       GET /api/ODataProducts?$filter=...       ┌─────────────────────┐
│  Browser  │──────────────────────────────────────────────►│     MeiShop          │
│  (Client) │                                               │  (API Gateway)       │
└──────────┘                                               │  Port 7024           │
                                                            └──────────┬───────────┘
                                                                       │
                                                    GET /odata/Products?$filter=...
                                                                       │
                                                                       ▼
                                                            ┌─────────────────────┐
                                                            │ ProductODataService  │
                                                            │ (OData v4 Endpunkt)  │
                                                            │ Port 7500            │
                                                            └─────────────────────┘
```

### Beispielabfragen

Direkt auf dem OData Service (Port 7500):

```bash
# Alle Produkte
curl -k https://localhost:7500/odata/Products

# Produkte teurer als 500€
curl -k "https://localhost:7500/odata/Products?\$filter=Price gt 500"

# Top 3 guenstigste Produkte
curl -k "https://localhost:7500/odata/Products?\$orderby=Price&\$top=3"

# Nur Laptops (Name und Preis)
curl -k "https://localhost:7500/odata/Products?\$filter=Category eq 'Laptops'&\$select=Name,Price"

# Paginierung: 3 Produkte ab Position 5
curl -k "https://localhost:7500/odata/Products?\$skip=4&\$top=3"

# Gesamtanzahl mit den ersten 2 Ergebnissen
curl -k "https://localhost:7500/odata/Products?\$count=true&\$top=2"

# Einzelnes Produkt mit Id 1
curl -k https://localhost:7500/odata/Products\(1\)

# OData Metadata-Dokument (EDM Schema)
curl -k https://localhost:7500/odata/\$metadata
```

Ueber den MeiShop Client (Port 7024):

```bash
# Alle Produkte ueber den API Gateway
curl -k https://localhost:7024/api/ODataProducts

# Gefiltert ueber den Gateway
curl -k "https://localhost:7024/api/ODataProducts?\$filter=Price gt 1000"
```

### Fazit

OData bietet im Kontext einer Microservice-Architektur einen standardisierten Weg, um flexible Datenabfragen zu ermoeglichen, ohne fuer jeden Anwendungsfall eigene Endpunkte implementieren zu muessen. Das implementierte Szenario demonstriert sowohl die **Server-Seite** (ProductODataService mit EDM Model und EnableQuery) als auch die **Client-Seite** (MeiShop OData Client Controller mit transparenter Query-Weiterleitung).
