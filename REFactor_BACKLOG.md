# Backend Refactor Backlog (UnitOfWork + OperationResult)

Bu dosya, tuple yasağı ve `OperationResult<T>` standardına geçiş için kalan işleri takip eder.

## Tamamlananlar

- `ApplicationService` ve `ContactService` tuple dönüşlerden `OperationResult<Guid>` modeline geçirildi.
- `ApplicationController` ve `ContactController` için ortak `OperationResult -> HTTP` mapper eklendi (`ControllerOperationResultExtensions`).
- `AdminController` create/update/save akışlarında `FromOperationResult` standardı kullanıldı; tekrar eden mapping kodu azaltıldı.
- `AdminDashboardService` içinde dosya indirme metotları tuple yerine `AdminAttachmentFileDto` dönecek şekilde güncellendi.
- `InvalidOperationException` kullanan admin create/update/save servis akışları `OperationResult<T>` modeline geçirildi.
- Startup seviyesinde servis sözleşmesi guard'ı eklendi (`ServiceContractGuard`), servis arayüzlerinde tuple dönüş tespit edilirse uygulama başlatılmaz.
- CI workflow eklendi: `dotnet build` + `--contract-check` smoke testi (`.github/workflows/backend-ci.yml`).

## Kalan İyileştirmeler

- Roslyn analyzer paketi ile tuple dönüş yasağını derleme aşamasında da zorunlu hale getirme (runtime guard'a ek güvence).
- Admin dışındaki kalan controller/service katmanlarında (varsa) aynı standardın tamamlanması.

