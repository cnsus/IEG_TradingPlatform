@echo off
echo Starte alle Microservices...

start "IEGEasyCreditcardService  (https://localhost:7231)" cmd /k "cd /d %~dp0IEGEasyCreditcardService && dotnet run --launch-profile https"
start "ProductService             (https://localhost:7200)" cmd /k "cd /d %~dp0ProductService && dotnet run --launch-profile https"
start "FtpProductCatalogService   (https://localhost:7300)" cmd /k "cd /d %~dp0FtpProductCatalogService && dotnet run --launch-profile https"
start "MeiShop                    (https://localhost:7024)" cmd /k "cd /d %~dp0MeiShop && dotnet run --launch-profile https"

echo Alle Services gestartet.
