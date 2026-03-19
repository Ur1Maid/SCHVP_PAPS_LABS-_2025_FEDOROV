# Лабораторная работа №5 — исходники

Папка содержит два типа артефактов:

- `reference/` — исходные конфигурации из реальных репозиториев проекта НВ:
  локальные `docker-compose`, `Dockerfile`, GitLab CI includes, DevOps templates и настройки развёртывания.
- `lab/` — итоговое решение для лабораторной:
  единый `docker-compose`, пример `.env`, скрипт сборки образов, шаблоны GitHub Actions и интеграционные smoke-тесты.

## Локальный запуск

1. Скопировать `.env.example` в `.env`.
2. При необходимости поправить пути до соседних репозиториев:
   - `automatic-dispatch-frontend`
   - `cumlist-app-service-develop`
   - `del-cumlist-data-service-develop`
   - `cumlist-normalize-service-develop`
3. Построить образы:
   ```bash
   ./build-images.sh
   ```
4. Поднять стенд:
   ```bash
   docker compose --env-file .env -f docker-compose.lab5.yml up -d
   ```
5. Проверить smoke-тесты:
   ```bash
   ./tests/run-newman.sh
   ```

## Важное замечание

Оригинальные backend Dockerfile и фронтенд-сборка используют корпоративные ресурсы:
- `auriga-repo.nts.local`
- приватный NuGet feed `nexus-nuget-nts`
- приватный npm registry

Поэтому лабораторный стенд полностью воспроизводим только при доступе к внутренней инфраструктуре.
Вне корпоративной сети потребуется зеркалирование базовых образов и пакетов.
