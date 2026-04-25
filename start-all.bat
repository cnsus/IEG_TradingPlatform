@echo off
echo ========================================
echo   IEG Trading Platform - Alle Services
echo ========================================
echo.

echo Starte alle Microservices...

REM 1. gRPC LoggingService
start "LoggingService (gRPC)          (http://localhost:5500)" cmd /k "cd /d %~dp0LoggingService && dotnet run"
timeout /t 2 /nobreak >nul

REM CreditcardService einmal bauen, damit die parallelen Instanzen sich nicht die bin/obj-Dateien gegenseitig sperren
echo Baue CreditcardService einmalig...
pushd "%~dp0IEGEasyCreditcardService"
dotnet build -v:minimal
popd

REM 2. CreditcardService Instanz 1 (Standard)
start "CreditcardService Instanz 1   (https://localhost:7231)" cmd /k "cd /d %~dp0IEGEasyCreditcardService && dotnet run --no-build --launch-profile https"

REM 3. CreditcardService Instanz 2
start "CreditcardService Instanz 2   (https://localhost:7232)" cmd /k "cd /d %~dp0IEGEasyCreditcardService && dotnet run --no-build --urls https://localhost:7232;http://localhost:5229"

REM 4. CreditcardService Instanz 3
start "CreditcardService Instanz 3   (https://localhost:7233)" cmd /k "cd /d %~dp0IEGEasyCreditcardService && dotnet run --no-build --urls https://localhost:7233;http://localhost:5230"

REM 5. ProductService
start "ProductService                (https://localhost:7200)" cmd /k "cd /d %~dp0ProductService && dotnet run --launch-profile https"

REM 6. FtpProductCatalogService
start "FtpProductCatalogService      (https://localhost:7300)" cmd /k "cd /d %~dp0FtpProductCatalogService && dotnet run --launch-profile https"

REM 7. PaymentService
start "PaymentService                (https://localhost:7400)" cmd /k "cd /d %~dp0PaymentService && dotnet run --launch-profile https"

REM 8. MeiShop (API Gateway) - zuletzt
start "MeiShop                       (https://localhost:7024)" cmd /k "cd /d %~dp0MeiShop && dotnet run --launch-profile https"

echo.
echo Alle Services gestartet.
echo.
echo   - LoggingService (gRPC):    http://localhost:5500
echo   - CreditcardService #1:     https://localhost:7231
echo   - CreditcardService #2:     https://localhost:7232
echo   - CreditcardService #3:     https://localhost:7233
echo   - ProductService:           https://localhost:7200
echo   - FtpProductCatalogService: https://localhost:7300
echo   - PaymentService:           https://localhost:7400
echo   - MeiShop (Swagger):        https://localhost:7024/swagger
