# Task: Implement Aufgabe 7 - Webhook-Subscriber

## 1. Task Summary
Implement a Webhook scenario for the IEG Trading Platform. The PaymentService acts as the webhook publisher (sending HTTP POST callbacks on payment creation), and a new WebhookSubscriberService acts as the subscriber (receiving and storing notifications).

## 2. Context Constraints
- DO follow the existing project patterns (Repository pattern, DI, controller-based API, German comments).
- DO use port 7600 for the new service (no conflicts with existing ports).
- DO NOT introduce new NuGet packages beyond what's already used.
- DO NOT break existing PaymentService functionality.
- DO write the documentation in German.

## 3. Dependencies & Prerequisites
None.

## 4. Affected Files Overview
**New Files (WebhookSubscriberService):**
- `WebhookSubscriberService/WebhookSubscriberService.csproj`
- `WebhookSubscriberService/Program.cs`
- `WebhookSubscriberService/Properties/launchSettings.json`
- `WebhookSubscriberService/appsettings.json`
- `WebhookSubscriberService/appsettings.Development.json`
- `WebhookSubscriberService/Models/PaymentNotification.cs`
- `WebhookSubscriberService/Services/INotificationRepository.cs`
- `WebhookSubscriberService/Services/NotificationRepository.cs`
- `WebhookSubscriberService/Controllers/WebhookController.cs`

**New Files (PaymentService webhook additions):**
- `PaymentService/Models/WebhookSubscription.cs`
- `PaymentService/Services/IWebhookService.cs`
- `PaymentService/Services/WebhookService.cs`
- `PaymentService/Controllers/WebhookSubscriptionsController.cs`

**Modified Files:**
- `PaymentService/Controllers/PaymentsController.cs` - Added webhook notification on payment creation
- `PaymentService/Program.cs` - Registered IWebhookService in DI
- `SolTradingPlatform.sln` - Added WebhookSubscriberService project
- `start-all.sh` - Added WebhookSubscriberService launch
- `start-all.bat` - Added WebhookSubscriberService launch
- `documentation/Aufgabe7.md` - Full theoretical + implementation documentation in German
- `documentation/0Aufgabenstellung.md` - Marked Aufgabe 7 as [x]
- `AG-WF-Artefacts/.ag-project-context.md` - Added WebhookSubscriberService

## 5. Step-by-Step Implementation Plan
1. Create WebhookSubscriberService with standard ASP.NET Core Web API structure
2. Add webhook subscription model and service to PaymentService
3. Modify PaymentsController.Create to fire webhooks
4. Register WebhookService in PaymentService DI
5. Add project to solution file
6. Update start scripts
7. Build and verify entire solution compiles
8. Write documentation in German
9. Update project context and assignment checklist

## 6. Acceptance Criteria
- [x] WebhookSubscriberService builds successfully (0 errors)
- [x] PaymentService builds successfully (0 errors)
- [x] Entire solution builds successfully (0 errors)
- [x] Documentation written in German under Ausarbeitung
- [x] Checkbox in 0Aufgabenstellung.md is ticked [x]
- [x] Project context updated with new service
- [x] Start scripts updated
