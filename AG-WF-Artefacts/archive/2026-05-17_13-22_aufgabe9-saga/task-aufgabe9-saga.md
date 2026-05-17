# Task: Implement Aufgabe 9 - SAGA-Pattern

## 1. Task Summary
Implement a SAGA Pattern scenario for the IEG Trading Platform. An OrderSagaService acts as an orchestrator coordinating an order placement workflow across ProductService and PaymentService, with compensating transactions on failure. Also write theoretical documentation in German.

## 2. Context Constraints
- DO follow existing project patterns (Repository, DI, controller-based API, German comments).
- DO use port 7700 for the new service.
- DO NOT introduce new NuGet packages.
- DO NOT break existing service functionality.
- DO write documentation in German.

## 3. Affected Files Overview
**New Files (OrderSagaService):**
- `OrderSagaService/OrderSagaService.csproj`
- `OrderSagaService/Program.cs`
- `OrderSagaService/Properties/launchSettings.json`
- `OrderSagaService/appsettings.json` / `appsettings.Development.json`
- `OrderSagaService/Models/Order.cs`, `PlaceOrderRequest.cs`, `SagaStep.cs`
- `OrderSagaService/Services/IOrderRepository.cs`, `OrderRepository.cs`
- `OrderSagaService/Services/IOrderSagaOrchestrator.cs`, `OrderSagaOrchestrator.cs`
- `OrderSagaService/Controllers/OrderSagaController.cs`

**Modified Files:**
- `SolTradingPlatform.sln`, `start-all.sh`, `start-all.bat`
- `documentation/Aufgabe9.md`, `documentation/0Aufgabenstellung.md`
- `AG-WF-Artefacts/.ag-project-context.md`

## 4. Acceptance Criteria
- [x] OrderSagaService builds successfully (0 errors)
- [x] Entire solution builds (0 errors, 0 warnings)
- [x] Documentation written in German
- [x] Checkbox in 0Aufgabenstellung.md is ticked [x]
- [x] Project context updated
- [x] Start scripts updated
