# Aufgabe 2 - Microservice Produktkataloge (10 Punkte)

## Aufgabenstellung

Erstellen Sie 2 weitere Microservice Produktkataloge: Erstellen Sie ein Microservice, welches eine Liste von Produkten anbietet. Der Inhalt der Liste soll dabei aus einem „microservice local datastore" kommen – (Decentralized Data Management). Ersetzen Sie die hard codierten Werte im MeiShop/ProductList-Controller durch den Aufruf des soeben erstellen Services. Ein weiterer Produktkatalog-Service soll Produkte aus einem Text File auf einem FTP-Server auslesen oder einem anderen geeigneten Persistencestore und zur Verfügung stellen. (10 Punkte)

## Ausarbeitung

Es wurden zwei separate Microservices implementiert:

**ProductService** (Port 7200): Verwaltet eine lokale Produktliste (Laptop, Smartphone, Tablet, Headphones, Smartwatch) ueber ein `ProductRepository` mit Dependency Injection. Endpoint: `GET /api/products` liefert die Produkte als JSON.

**FtpProductCatalogService** (Port 7300): Liest dynamisch eine `products.txt`-Datei von einem FTP-Server via `FluentFTP` (mit FTPS und selbstsigniertem Zertifikat). Endpoint: `GET /api/productcatalog` liefert die Produkte asynchron als JSON.

**MeiShop-Integration**: Die hardcodierten Werte im `ProductListController` und `ProductCatalogController` wurden durch HTTP-Aufrufe an die beiden neuen Services ersetzt. Die Service-URLs sind in `appsettings.json` konfiguriert, mit strukturierter Fehlerbehandlung und Fallback-Meldungen.

### Dateien

| Datei | Beschreibung |
|---|---|
| `ProductService/Controllers/ProductsController.cs` | REST-Controller fuer Produkte |
| `ProductService/Services/ProductRepository.cs` | Lokaler Datenstore (In-Memory) |
| `FtpProductCatalogService/Controllers/ProductCatalogController.cs` | REST-Controller fuer FTP-Katalog |
| `FtpProductCatalogService/Services/FtpProductCatalogRepository.cs` | FTP-Zugriff via FluentFTP |
| `MeiShop/Controllers/ProductListController.cs` | Ruft ProductService auf |
| `MeiShop/Controllers/ProductCatalogController.cs` | Ruft FtpProductCatalogService auf |
