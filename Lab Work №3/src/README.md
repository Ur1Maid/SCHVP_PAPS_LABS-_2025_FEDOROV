# Исходные фрагменты кода для лабораторной работы №3

В этой папке собраны **реальные фрагменты** из проектных репозиториев модуля «Накопительная ведомость», на основе которых подготовлен отчёт.

## Источники

### Frontend
Архив: `automatic-dispatch-frontend.zip`  
Использованы фрагменты из `packages/apps/cumlist/src/...`

Ключевые файлы:
- `shared/hooks/useSignCumList.tsx`
- `shared/components/ModalRejectionReason/useRejectCumList.tsx`
- `shared/hooks/useCumListSubscription.ts`
- `shared/hooks/useAllCumListOperationsSubscription.ts`
- `shared/hooks/useCumList.tsx`
- `features/CumListsTable/components/CumListsPage.tsx`
- `features/CumListsTable/hooks/useCumListPage.tsx`
- `features/CumListCard/components/CardActionsButtons.tsx`
- GraphQL mutations / queries / subscriptions

### Backend - командная часть
Архив: `cumlist-app-service-develop.zip`

Ключевые файлы:
- `Services/GraphQL/Mutation.cs`
- `Handlers/Core/BaseCumListOperationHandler.cs`
- `Handlers/SignOperationCumListHandler.cs`
- `Handlers/RejectOperationCumListHandler.cs`
- `Kafka/Handlers/CumListOperationsTopicHandler.cs`
- `Handlers/IntegrationModuleReplyHandler.cs`
- `Handlers/NormalizedDocHandler.cs`

### Backend - нормализация
Архив: `cumlist-normalize-service-develop.zip`

Ключевые файлы:
- `Handlers/NormalizeDocHandler.cs`
- `Handlers/NormalizeSyncHandler.cs`
- `Handlers/NormalizeUpdateHandler.cs`
- `Mappers/MapperFactory.cs`
- `Database/Models/CumList.cs`
- `Database/Models/CumListDue.cs`
- `Database/ViewFactory.cs`

### Backend - read model / queries
Архив: `del-cumlist-data-service-develop.zip`

Ключевые файлы:
- `Services/GraphQL/Query.CumListView.cs`
- `Services/GraphQL/Query.CumListDue.cs`
- `schema.graphql`

## Назначение папки

Эти файлы нужны не как самостоятельный новый проект, а как **доказательная база** для отчёта:
- для привязки диаграмм к реальному коду;
- для показа принципов KISS, YAGNI, DRY, SOLID;
- для демонстрации реального клиентского и серверного взаимодействия по сценарию sign/reject НВ.
