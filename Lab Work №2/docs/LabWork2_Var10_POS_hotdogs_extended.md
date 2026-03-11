# Лабораторная работа №2

**Тема:** Использование нотации C4 model для проектирования архитектуры программной системы  
**Проект:** система точек продаж для киосков с хот-догами (POS HotDog)  
**Цель работы:** Получить опыт использования графической нотации C4 для фиксации архитектурных решений на примере системы POS для сети киосков с хот-догами.

---

## 1. Диаграмма системного контекста (C4: System Context)

Ниже представлена диаграмма системного контекста, отражающая ключевых стейкхолдеров и внешние программные системы, с которыми взаимодействует система POS.

![ C4 — System Context — POS HotDog](./C4_system_context.png)

---

## 2. Диаграмма контейнеров (C4: Container)

Диаграмма контейнеров показывает основные приложения и сервисы, из которых состоит система POS HotDog, и их взаимодействия между собой и с внешними системами.

![ C4 — Container — POS HotDog](./C4_container.png)

---

## 3. Диаграммы компонентов backend (C4: Component — Backend)

### 3.1. Общая компонентная структура backend

Первая диаграмма показывает основные компоненты внутри контейнера POS Backend API.

![ C4 — Components — POS Backend API](./C4_components__Backend_API.png)

### 3.2. Backend: управление запасами и логистикой

Вторая диаграмма детализирует компоненты, связанные с запасами и заявками на пополнение.

![ C4 — Components — Inventory & Logistics](./C4_components_inventory&logistics.png)

---

## 4. Диаграммы компонентов frontend (C4: Component — Frontend)

### 4.1. Mobile POS App — компоненты

Диаграмма показывает основные экраны и служебные модули мобильного приложения оператора.

![ C4 — Components — Mobile POS App](./C4_components_mobile_POS_App.png)

---

### 4.2. Inventory App — компоненты

![ C4 — Components — Inventory App](./C4_components_inventory_app.png)

---

### 4.3. Back-office Web Portal — компоненты

![ C4 — Components — Back-office Web Portal](./C4_components_back_office_web_portal.png)

---

## 5. Диаграмма компонентов интеграций (соцсети и бухгалтерия)

Последняя диаграмма детализирует взаимодействие backend с адаптерами и внешними системами.

![  C4 — Components — Integrations (Social & Accounting)](./C4_components_integrations.png)

---

Таким образом, в отчёт включено 8 диаграмм на основе C4-подхода:

1. System Context
2. Container
3. Backend — общий компонентный вид
4. Backend — управление запасами и логистикой
5. Frontend — Mobile POS App
6. Frontend — Inventory App
7. Frontend — Back-office Web Portal
8. Интеграции — соцсети и бухгалтерия

Код каждой диаграммы приведён в формате PlantUML, что позволяет легко получить PNG-изображения для отчёта или презентации.
