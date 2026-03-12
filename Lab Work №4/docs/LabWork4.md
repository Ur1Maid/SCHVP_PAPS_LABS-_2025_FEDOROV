# Лабораторная работа №4

**Тема:** Проектирование API  
**Проект:** модуль «Накопительная ведомость»  
**Формат выполнения:** адаптация задания под реальный проект, в котором внешний контракт реализован через **GraphQL**, а не через REST.

## 0. Обоснование выбранного подхода

По формулировке лабораторной работы №4 требуется спроектировать и задокументировать REST API. Однако для модуля НВ в проектной и функциональной документации явно зафиксировано, что модуль должен публиковать данные через **GraphQL API**, а взаимодействие со смежными подсистемами должно осуществляться с использованием **GraphQL**, системной шины и центра данных. Поэтому в данной лабораторной работе используется не учебный искусственный REST-контракт, а реальный контракт проекта — GraphQL API модуля НВ.

В результате лабораторная работа остаётся привязанной к реальному коду проекта и его фактической архитектуре:
- `DEL_cumlist-data-service` — публичный слой чтения;
- `cumlist-app-service` — публичный слой командных операций;
- `cumlist-normalize-service` — внутренний интеграционный слой нормализации, использующий GraphQL-запросы к внешним источникам, но не публикующий основной пользовательский API напрямую.

Дополнительно для уточнения общего корневого GraphQL-контракта была использована `coreSchema.graphql`, но не как основной источник по НВ, а только как проверочный: она подтверждает наличие общего `Subscription` и точную сигнатуру `entityOperationResult(correlationId: UUID!): String!`. Основной набор Query и Mutation по-прежнему берётся из cumlist-специфичных сервисов.

---

## 1. Выбранный сервис и границы API

В рамках лабораторной рассматривается GraphQL API модуля НВ, которое обслуживает два основных пользовательских сценария:

1. **Чтение данных**
   - журнал накопительных ведомостей;
   - карточка НВ;
   - журнал сборов;
   - получение одного сбора по идентификатору.

2. **Командные операции**
   - подписание НВ;
   - отклонение НВ.

### 1.1. Публичные операции, включённые в лабораторную

| № | Операция | Тип GraphQL | Реальный источник |
|---|---|---|---|
| 1 | `cumlistCumLists` | Query | `DEL_cumlist-data-service` |
| 2 | `cumlistCumListByDocId` | Query | `DEL_cumlist-data-service` |
| 3 | `cumlistCumListDues` | Query | `DEL_cumlist-data-service` |
| 4 | `cumlistCumListDueById` | Query | `DEL_cumlist-data-service` |
| 5 | `cumlistSignDocument` | Mutation | `cumlist-app-service` |
| 6 | `cumlistRejectDocument` | Mutation | `cumlist-app-service` |

### 1.2. Дополнительная операция проекта

В реальном фронтенде также используется подписка:

- `entityOperationResult(correlationId: UUID!): String!`

Её сигнатура дополнительно подтверждена общей `coreSchema.graphql`. Подписка нужна для отслеживания результата асинхронной операции после вызова мутации. В отчёт она включена как важная часть проектного решения, но базовый набор тестов в Postman построен на шести основных операциях из таблицы выше.

---

## 2. Принятые проектные решения при проектировании API

### 2.1. Разделение API на слой чтения и слой команд
В проекте чтение и изменение состояния документа не смешиваются в одном сервисе.  
`DEL_cumlist-data-service` отвечает за Query-операции, а `cumlist-app-service` — за Mutation-операции. Это упрощает сопровождение и соответствует CQRS-подходу на уровне внешнего контракта.

### 2.2. Использование GraphQL вместо набора разрозненных REST endpoint’ов
Модуль НВ должен отдавать разные срезы данных: журнал, карточку, сборы, историю, справочники, вложенные сущности. GraphQL позволяет запрашивать только нужные поля, а фронтенд уже использует именно такой контракт.

### 2.3. Явное разделение Query и Mutation
Операции чтения оформлены через `Query`, а действия, меняющие состояние документов, — через `Mutation`. Это делает семантику API прозрачной и не смешивает выборку данных с бизнес-действиями.

### 2.4. Асинхронные командные операции через `correlationId`
Подписание и отклонение документа не завершаются мгновенно в рамках одного HTTP-ответа. В мутациях используется `correlationId`, который позволяет связать команду с последующим результатом обработки.

### 2.5. Отслеживание результата через подписку
После вызова мутации фронтенд подписывается на `entityOperationResult(correlationId)`. Такое решение лучше подходит для сценария, в котором операция публикуется в Kafka и завершается после ответа интеграционного модуля. Общая `coreSchema.graphql` подтверждает, что эта подписка определена на уровне корневого `Subscription` и возвращает `String!`, то есть пользовательский слой НВ опирается не на локально выдуманный канал, а на общий контракт платформы.

### 2.6. Сильная типизация входных данных
Для мутаций используются отдельные input-типы:
- `CumlistSignDocumentInput`
- `CumlistRejectDocumentInput`

Это упрощает валидацию, делает контракт самодокументируемым и уменьшает риск неоднозначности параметров.

### 2.7. Сильная типизация выходных данных
Даже простые командные операции возвращают типизированные payload-объекты:
- `CumlistSignDocumentPayload`
- `CumlistRejectDocumentPayload`

За счёт этого API можно расширять без ломающих изменений, например добавлением полей статуса, ошибок или дополнительного контекста.

### 2.8. Пагинация для журнальных выборок
Для операций списка используются `skip` и `take`, а также возвращается `pageInfo` и `totalCount`. Это нужно из-за потенциально больших журналов НВ и сборов.

### 2.9. Фильтрация и сортировка на стороне API
Операции `cumlistCumLists` и `cumlistCumListDues` поддерживают `where` и `order`. Это позволяет выполнять глобальную фильтрацию и выборки без дублирования логики на фронтенде.

### 2.10. Контракт строится вокруг идентификаторов документа и сбора
Ключевые точки входа в API — это `docId` для НВ и `id` для сбора. Такой контракт соответствует предметной области и пользовательским сценариям перехода из журнала в карточку.

### 2.11. Внутренний нормализующий слой отделён от публичного API
`cumlist-normalize-service` тоже использует GraphQL, но только для межсервисной синхронизации и подготовки нормализованных данных. Он не смешивается с пользовательским публичным контрактом.

### 2.12. Минимизация количества round-trip на клиенте
Фронтенд получает журнал, карточку и сборы через запросы с нужным набором полей, а командные операции — через отдельные мутации. Это упрощает orchestration на клиенте и снижает избыточные запросы.

---

## 3. Документация по API

### Общие сведения о транспорте

В отличие от REST-подхода, в GraphQL почти все вызовы выполняются через `POST /graphql`, а различие между операциями определяется телом запроса:
- `operationName`
- `query`
- `variables`

**Типовой HTTP-запрос:**
```http
POST /graphql
Content-Type: application/json
Authorization: Bearer <token>
```

**Типовой формат тела:**
```json
{
  "operationName": "cumLists",
  "query": "query cumLists($take: Int, $skip: Int, $where: CumlistCumListFilterInput, $requestDate: DateTime!) { ... }",
  "variables": {
    "take": 20,
    "skip": 0,
    "requestDate": "2026-03-12T00:00:00Z"
  }
}
```

---

### 3.1. Операция `cumlistCumLists`

**Тип:** Query  
**Сервис:** `DEL_cumlist-data-service`  
**Назначение:** получить страницу журнала накопительных ведомостей.

**GraphQL-операция:**
```graphql
query cumLists(
  $take: Int
  $skip: Int
  $where: CumlistCumListFilterInput
  $requestDate: DateTime!
) {
  cumlistCumLists(take: $take, skip: $skip, where: $where) {
    totalCount
    items {
      docId
      number
      createDate
      startDate
      finishDate
      person
      contractor
      amountSum
      amountTotal
      taxValueSum
      dueCount
      state {
        id
      }
      docType {
        id
      }
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
    }
  }
}
```

**Переменные:**
```json
{
  "take": 20,
  "skip": 0,
  "requestDate": "2026-03-12T00:00:00Z",
  "where": {
    "stateId": { "eq": 2 }
  }
}
```

**Формат ответа:**
```json
{
  "data": {
    "cumlistCumLists": {
      "totalCount": 125,
      "items": [
        {
          "docId": 100500,
          "number": "НВ-24/0001",
          "createDate": "2024-12-10T09:00:00Z",
          "amountTotal": 10500.25,
          "dueCount": 4,
          "state": { "id": 2 },
          "docType": { "id": 18 }
        }
      ],
      "pageInfo": {
        "hasNextPage": true,
        "hasPreviousPage": false
      }
    }
  }
}
```

**Назначение на фронтенде:** заполнение журнала НВ, фильтрация и постраничная навигация.

---

### 3.2. Операция `cumlistCumListByDocId`

**Тип:** Query  
**Сервис:** `DEL_cumlist-data-service`  
**Назначение:** получить карточку одной накопительной ведомости по идентификатору документа.

**GraphQL-операция:**
```graphql
query cumDetails($requestDate: DateTime!, $docId: Long) {
  cumlistCumListByDocId(docId: $docId) {
    docId
    mainId
    number
    createDate
    startDate
    finishDate
    person
    contractor
    discord
    arbSign
    arbNum
    clientId
    payerId
    orgId
    typeId
    stationId
    payFormId
    payPlaceId
    amountSum
    amountTotal
    taxValueSum
    dueCount
    state {
      id
    }
    docType {
      id
    }
  }
}
```

**Переменные:**
```json
{
  "docId": 100500,
  "requestDate": "2026-03-12T00:00:00Z"
}
```

**Формат ответа:**
```json
{
  "data": {
    "cumlistCumListByDocId": {
      "docId": 100500,
      "number": "НВ-24/0001",
      "person": "Иванов И.И.",
      "contractor": "ООО Ромашка",
      "amountTotal": 10500.25,
      "dueCount": 4,
      "state": { "id": 2 }
    }
  }
}
```

**Назначение на фронтенде:** загрузка вкладки «Документ» в карточке НВ.

---

### 3.3. Операция `cumlistCumListDues`

**Тип:** Query  
**Сервис:** `DEL_cumlist-data-service`  
**Назначение:** получить список сборов. В карточке НВ обычно используется фильтрация по `docId`.

**GraphQL-операция:**
```graphql
query duesByCumListId($requestDate: DateTime!, $docId: Long) {
  cumlistCumListDues(where: { docId: { eq: $docId } }) {
    items {
      id
      dueDate
      parentDocNum
      amount
      taxValue
      kzAmount
      kzTaxValue
      info
      note
      existSign
      parentDocId
      docId
      dueTypeId
      parentDocTypeId
    }
    totalCount
  }
}
```

**Переменные:**
```json
{
  "docId": 100500,
  "requestDate": "2026-03-12T00:00:00Z"
}
```

**Формат ответа:**
```json
{
  "data": {
    "cumlistCumListDues": {
      "items": [
        {
          "id": "2a70f8c1-55e5-4ad2-9a58-cf3988b8f8d1",
          "docId": 100500,
          "amount": 1200.50,
          "taxValue": 240.10,
          "note": "Маневровая работа"
        }
      ],
      "totalCount": 1
    }
  }
}
```

**Назначение на фронтенде:** заполнение блока «Сборы» в карточке НВ и журнала сборов.

---

### 3.4. Операция `cumlistCumListDueById`

**Тип:** Query  
**Сервис:** `DEL_cumlist-data-service`  
**Назначение:** получить один сбор по его идентификатору.

**GraphQL-операция:**
```graphql
query dueById($id: UUID!) {
  cumlistCumListDueById(id: $id) {
    id
    dueDate
    parentDocNum
    amount
    taxValue
    kzAmount
    kzTaxValue
    info
    note
    existSign
    agMpsOrgId
    parentDocId
    docId
    dueTypeId
    parentDocTypeId
  }
}
```

**Переменные:**
```json
{
  "id": "2a70f8c1-55e5-4ad2-9a58-cf3988b8f8d1"
}
```

**Формат ответа:**
```json
{
  "data": {
    "cumlistCumListDueById": {
      "id": "2a70f8c1-55e5-4ad2-9a58-cf3988b8f8d1",
      "docId": 100500,
      "amount": 1200.50,
      "taxValue": 240.10,
      "note": "Маневровая работа"
    }
  }
}
```

---

### 3.5. Операция `cumlistSignDocument`

**Тип:** Mutation  
**Сервис:** `cumlist-app-service`  
**Назначение:** инициировать подписание накопительной ведомости.

**GraphQL-операция:**
```graphql
mutation signCumList($input: CumlistSignDocumentInput!) {
  cumlistSignDocument(input: $input) {
    long
  }
}
```

**Входной тип:**
```json
{
  "input": {
    "docId": 100500,
    "correlationId": "5f6b845e-1cab-48a8-a9bb-7ef1c0878e6b"
  }
}
```

**Формат ответа:**
```json
{
  "data": {
    "cumlistSignDocument": {
      "long": 100500
    }
  }
}
```

**Особенности:**
- мутация не гарантирует немедленного завершения бизнес-процесса;
- `correlationId` используется для последующего отслеживания результата обработки.

---

### 3.6. Операция `cumlistRejectDocument`

**Тип:** Mutation  
**Сервис:** `cumlist-app-service`  
**Назначение:** инициировать отклонение накопительной ведомости.

**GraphQL-операция:**
```graphql
mutation rejectCumList($input: CumlistRejectDocumentInput!) {
  cumlistRejectDocument(input: $input) {
    long
  }
}
```

**Входной тип:**
```json
{
  "input": {
    "docId": 100500,
    "discordId": 1,
    "discordText": "Найдены расхождения по начислениям",
    "correlationId": "5f6b845e-1cab-48a8-a9bb-7ef1c0878e6b"
  }
}
```

**Формат ответа:**
```json
{
  "data": {
    "cumlistRejectDocument": {
      "long": 100500
    }
  }
}
```

**Особенности:**
- используется причина отклонения и произвольный комментарий;
- реальный UI получает справочник причин отдельно и передаёт выбранное значение в мутацию.

---

### 3.7. Дополнительная операция `entityOperationResult`

**Тип:** Subscription  
**Источник использования:** frontend / общий GraphQL-контур  
**Назначение:** получить финальный результат асинхронной операции по `correlationId`.

**GraphQL-операция:**
```graphql
subscription EntityOperationResult($correlationId: UUID!) {
  entityOperationResult(correlationId: $correlationId)
}
```

**Переменные:**
```json
{
  "correlationId": "5f6b845e-1cab-48a8-a9bb-7ef1c0878e6b"
}
```

**Роль в проекте:** связка между mutation-запросом и фактическим завершением бизнес-операции после ответа интеграционного модуля.

---

## 4. Тестирование API в Postman

Для лабораторной подготовлена коллекция Postman:

- `src/postman/CumList GraphQL API.postman_collection.json`
- `src/postman/CumList GraphQL API.postman_environment.json`

Коллекция содержит шесть готовых запросов по реальным операциям модуля НВ.

### 4.1. Переменные окружения Postman

| Переменная | Назначение |
|---|---|
| `dataServiceUrl` | URL GraphQL endpoint сервиса чтения |
| `appServiceUrl` | URL GraphQL endpoint сервиса команд |
| `requestDate` | дата запроса для lookup-полей |
| `docId` | идентификатор накопительной ведомости |
| `dueId` | идентификатор сбора |
| `correlationId` | идентификатор корреляции для mutation |
| `discordId` | идентификатор причины отклонения |
| `discordText` | комментарий при отклонении |
| `token` | bearer token, если он нужен в окружении |

### 4.2. Набор тестов

Для каждого запроса в коллекции подготовлены минимум два автотеста:

1. проверка, что HTTP-ответ получен успешно;
2. проверка, что в ответе нет `errors` и присутствует ожидаемое поле `data`.

Дополнительно в отдельных операциях проверяется наличие ключевого поля результата:
- `totalCount` для журналов;
- `docId` или `id` для выборки сущности;
- `long` для мутаций.

### 4.3. Что именно нужно приложить в репозиторий после локального запуска

Требование лабораторной работы просит показать принтскрины из Postman. Поэтому после локального запуска сервисов нужно сделать скриншоты:
- тела запроса;
- вкладок Headers / Body;
- полученного ответа;
- блока Test Results.

В текущем комплекте я подготовил:
- сам отчёт;
- коллекцию;
- окружение;
- тестовые скрипты.

**Скриншоты должны быть сняты уже в твоём локальном окружении**, потому что без запуска конкретных сервисов и без GUI Postman их нельзя получить честно и правдоподобно.

---

## 5. Как этот API связан с реальным кодом проекта

### 5.1. Frontend
Во фронтенде реально присутствуют GraphQL-операции:
- `cumLists`
- `cumDetails`
- `duesByCumListId`
- `signCumList`
- `rejectCumList`
- `EntityOperationResult`

### 5.2. `DEL_cumlist-data-service`
Сервис реализует Query-операции:
- `GetCumLists`
- `GetCumListByDocIdAsync`
- `GetCumListDues`
- `GetCumListDueByIdAsync`

### 5.3. `cumlist-app-service`
Сервис реализует Mutation-операции:
- `SignDocumentAsync`
- `RejectDocumentAsync`

### 5.4. `cumlist-normalize-service`
Сервис не используется как публичный пользовательский API, но подтверждает, что GraphQL — не случайный учебный выбор, а один из базовых интеграционных механизмов проекта: он строит запросы к внешним источникам и наполняет нормализованное хранилище.

---

## 6. Вывод

В данной лабораторной работе зафиксирован и задокументирован **реальный GraphQL API-контракт** модуля «Накопительная ведомость», используемый во фронтенде и backend-сервисах проекта. Несмотря на то, что формулировка задания ориентирована на REST, адаптация под GraphQL является обоснованной, потому что именно такой интерфейс предусмотрен архитектурой и функциональными требованиями модуля НВ.

В отчёт включены:
- не менее 8 проектных решений;
- документация по 6 операциям API;
- подготовленные артефакты для тестирования в Postman;
- привязка к реальным сервисам и исходному коду проекта.
