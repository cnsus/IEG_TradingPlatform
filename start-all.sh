#!/bin/bash
# ============================================================
# Aufgabe 3: Start-Skript fuer alle Microservices (macOS/Linux)
# ============================================================
# Startet:
# - gRPC LoggingService (Port 5500)
# - 3x IEGEasyCreditcardService (Ports 7231, 7232, 7233)
# - ProductService (Port 7200)
# - FtpProductCatalogService (Port 7300)
# - PaymentService (Port 7400)
# - ProductODataService (Port 7500) — Aufgabe 8: OData
# - MeiShop API Gateway (Port 7024)
# ============================================================

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

echo "========================================"
echo "  IEG Trading Platform - Alle Services"
echo "========================================"
echo ""

# 1. gRPC LoggingService starten
echo "[1/9] Starte gRPC LoggingService (http://localhost:5500)..."
(cd "$SCRIPT_DIR/LoggingService" && dotnet run) &
sleep 2

# 2. CreditcardService Instanz 1 (Standard-Port)
echo "[2/9] Starte CreditcardService Instanz 1 (https://localhost:7231)..."
(cd "$SCRIPT_DIR/IEGEasyCreditcardService" && dotnet run --launch-profile https) &
sleep 1

# 3. CreditcardService Instanz 2
echo "[3/9] Starte CreditcardService Instanz 2 (https://localhost:7232)..."
(cd "$SCRIPT_DIR/IEGEasyCreditcardService" && dotnet run --urls "https://localhost:7232;http://localhost:5229") &
sleep 1

# 4. CreditcardService Instanz 3
echo "[4/9] Starte CreditcardService Instanz 3 (https://localhost:7233)..."
(cd "$SCRIPT_DIR/IEGEasyCreditcardService" && dotnet run --urls "https://localhost:7233;http://localhost:5230") &
sleep 1

# 5. ProductService
echo "[5/9] Starte ProductService (https://localhost:7200)..."
(cd "$SCRIPT_DIR/ProductService" && dotnet run --launch-profile https) &
sleep 1

# 6. FtpProductCatalogService
echo "[6/9] Starte FtpProductCatalogService (https://localhost:7300)..."
(cd "$SCRIPT_DIR/FtpProductCatalogService" && dotnet run --launch-profile https) &
sleep 1

# 7. PaymentService
echo "[7/9] Starte PaymentService (https://localhost:7400)..."
(cd "$SCRIPT_DIR/PaymentService" && dotnet run --launch-profile https) &
sleep 1

# 8. ProductODataService (Aufgabe 8: OData)
echo "[8/9] Starte ProductODataService (https://localhost:7500)..."
(cd "$SCRIPT_DIR/ProductODataService" && dotnet run --launch-profile https) &
sleep 1

# 9. MeiShop (API Gateway) – zuletzt, damit alle Backend-Services bereit sind
echo "[9/9] Starte MeiShop API Gateway (https://localhost:7024)..."
(cd "$SCRIPT_DIR/MeiShop" && dotnet run --launch-profile https) &

echo ""
echo "========================================"
echo "  Alle Services gestartet!"
echo "========================================"
echo ""
echo "  Services:"
echo "    - LoggingService (gRPC):    http://localhost:5500"
echo "    - CreditcardService #1:     https://localhost:7231"
echo "    - CreditcardService #2:     https://localhost:7232"
echo "    - CreditcardService #3:     https://localhost:7233"
echo "    - ProductService:           https://localhost:7200"
echo "    - FtpProductCatalogService: https://localhost:7300"
echo "    - PaymentService:           https://localhost:7400"
echo "    - ProductODataService:      https://localhost:7500/odata/Products"
echo "    - MeiShop (Swagger):        https://localhost:7024/swagger"
echo ""
echo "  Zum Stoppen: Ctrl+C oder 'kill %' fuer einzelne Jobs"
echo "========================================"

# Auf alle Hintergrund-Prozesse warten
wait
