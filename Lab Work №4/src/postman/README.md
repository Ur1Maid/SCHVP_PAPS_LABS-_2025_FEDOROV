# Postman

Файлы:
- `CumList GraphQL API.postman_collection.json`
- `CumList GraphQL API.postman_environment.json`

## Как использовать

1. Импортируй коллекцию и environment в Postman.
2. Заполни реальные значения:
   - `dataServiceUrl`
   - `appServiceUrl`
   - `docId`
   - `dueId`
   - `token` (если требуется)
3. Запусти по очереди 6 операций.
4. Для отчёта по лабораторной сохрани скриншоты:
   - тело запроса;
   - Headers / Body;
   - ответ;
   - Test Results.


## Что не входит в коллекцию

Подписка `entityOperationResult` в стандартную коллекцию не включена, так как для неё обычно требуется WebSocket/SSE-совместимый клиент или отдельная настройка Postman GraphQL Subscriptions. В отчёте она описана как часть реального контракта, но базовый прогон для лабораторной построен на 6 основных Query/Mutation.
