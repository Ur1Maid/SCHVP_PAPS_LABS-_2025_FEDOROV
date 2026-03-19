# Интеграционные smoke-тесты

В папке лежат два набора тестов:

1. `CumList Smoke Tests.postman_collection.json` — минимальная проверка контейнерного стенда:
   - доступность фронтенда;
   - health endpoint `cumlist-data-service`;
   - health endpoint `cumlist-app-service`;
   - health endpoint `cumlist-normalize-service`.

2. `CumList GraphQL API.postman_collection.json` — коллекция из лабораторной работы №4.
   Её можно подключать поверх этого же контейнерного стенда после подготовки тестовых данных в БД.

Базовая команда запуска:
```bash
./run-newman.sh
```
