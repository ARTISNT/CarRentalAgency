# CarRentalAgency

Микросервисная платформа аренды автомобилей. Бэкенд на .NET (микросервисы + API Gateway), фронтенд на React + TypeScript + Vite. Инфраструктура (брокер сообщений, СУБД, SMTP для разработки) поднимается в Docker.

## Архитектура

### Backend
- `CarService` — каталог и состояние автомобилей.
- `UserService` — пользователи, аутентификация, паспортные данные, верификация email.
- `RentalService` — аренды, расчёт стоимости, продление, завершение, штрафы.
- `ContractService` — договоры и шаблоны договоров (PDF, подпись).
- `PaymentService` — платежи через bePaid (депозит, полная оплата, штрафы, возвраты).
- `NotificationService` — отправка email-уведомлений через RabbitMQ.
- `Gateway` (YARP) — единая точка входа для фронтенда, маршрутизация в микросервисы.

### Инфраструктура (Docker)
- RabbitMQ + Management UI — брокер сообщений.
- MailHog — тестовый SMTP-сервер для разработки (письма не уходят наружу).
- 5 инстансов MS SQL Server 2022 — отдельная БД под каждый сервис (`UserDb`, `RentalDb`, `CarDb`, `ContractDb`, `PaymentDb`).

### Frontend
- React 19 + TypeScript + Vite.
- Ant Design, React Query, React Hook Form + Zod, Zustand.
- Проксирование `/api` на Gateway настроено в `vite.config.ts`.

## Требования

- Docker и Docker Compose.
- Node.js 18+ и npm.
- .NET SDK 8 — только если нужно собирать/запускать сервисы вне Docker (опционально).

## Конфигурация

Все секреты и параметры окружения хранятся в `Backend/.env` (уже присутствует со значениями по умолчанию для локальной разработки). Для production значения необходимо заменить.

Основные переменные:
- `MSSQL_PASSWORD` — пароль `sa` для всех SQL-инстансов.
- `RABBITMQ_DEFAULT_USER` / `RABBITMQ_DEFAULT_PASS` — учётные данные RabbitMQ.
- `InternalJWT_KEY`, `InternalJWT_ISSUER`, `InternalJWT_AUDIENCE` — JWT для межсервисного взаимодействия.
- `UserJWT_KEY`, `UserJWT_ISSUER`, `UserJWT_AUDIENCE` — пользовательские JWT.
- `EMAIL_VERIFICATION_SECRET` — секрет токенов подтверждения email.
- `APP_VERIFICATION_URL`, `APP_FRONTEND_BASE_URL` — URL фронтенда, подставляемый в письма.
- `SMTP_HOST`, `SMTP_PORT`, `SMTP_USE_SSL`, `SMTP_USERNAME`, `SMTP_PASSWORD`, `SMTP_FROM_ADDRESS`, `SMTP_FROM_NAME`, `SMTP_TO_ADDRESS` — параметры SMTP. В режиме разработки используется MailHog (`smtp:1025` внутри сети Docker), внешний SMTP не обязателен.
- `ContractCertificate_PASSWORD` — пароль для PDF-сертификатов договоров.
- `BEPAID_SHOP_ID`, `BEPAID_SECRET_KEY`, `BEPAID_CALLBACK_URL`, `BEPAID_NOTIFICATION_URL` — параметры платёжного шлюза bePaid. По умолчанию используется тестовый шлюз. Для приёма вебхуков извне понадобится туннель (например, ngrok) — задайте его адрес в `BEPAID_NOTIFICATION_URL`.

## Запуск бэкенда

```bash
cd Backend
docker compose up -d --build
```

Будут подняты: RabbitMQ, MailHog, 5 SQL-инстансов, 6 микросервисов и Gateway. Первый запуск занимает больше времени (скачивание образов SQL Server и сборка сервисов).

### Порты

| Сервис               | Порт       |
|----------------------|------------|
| API Gateway          | `5000`     |
| CarService           | `5001`     |
| UserService          | `5002`     |
| RentalService        | `5003`     |
| ContractService      | `5004`     |
| NotificationService  | `5005`     |
| PaymentService       | `5006`     |
| RabbitMQ AMQP        | `5672`     |
| RabbitMQ Management  | `15672`    |
| MailHog SMTP         | `1025`     |
| MailHog Web UI       | `8025`     |
| SQL: UserDb          | `1433`     |
| SQL: RentalDb        | `1434`     |
| SQL: CarDb           | `1435`     |
| SQL: ContractDb      | `1436`     |
| SQL: PaymentDb       | `1437`     |

Логи: `docker compose logs -f <service>`.
Остановка: `docker compose down`. Полная очистка с удалением данных БД: `docker compose down -v`.

## Запуск фронтенда

```bash
cd Frontend
npm install
npm run dev
```

Фронтенд будет доступен на `http://localhost:5173`. Все запросы на `/api/*` автоматически проксируются на Gateway (`http://localhost:5000`).

Доступные скрипты:
- `npm run dev` — запуск в режиме разработки.
- `npm run build` — продакшен-сборка.
- `npm run preview` — локальный просмотр продакшен-сборки.
- `npm run lint` — проверка ESLint.

## Миграции и служебные скрипты

В `Backend/Scripts/` находится `fix-missing-columns.sql` — скрипт, добавляющий недостающие колонки в существующие таблицы. Перед накатыванием убедитесь, что контейнеры с нужной БД подняты и здоровы. Пример запуска для `UserDb`:

```bash
docker exec -i UserDb /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_PASSWORD" -C -i /dev/stdin < Backend/Scripts/fix-missing-columns.sql
```

Для других БД замените имя контейнера (`RentalDb`, `CarDb`, `ContractDb`, `PaymentDb`). Параметр `-C` нужен для доверия самоподписанному сертификату SQL Server 2022.

## Веб-интерфейсы

- Фронтенд приложения: `http://localhost:5173`.
- API Gateway (Swagger отдельных сервисов доступен на своих портах): `http://localhost:5000`.
- RabbitMQ Management: `http://localhost:15672` (учётные данные из `.env`, по умолчанию `admin` / `admin123`).
- MailHog Web UI: `http://localhost:8025` — все исходящие письма, отправленные `NotificationService` в режиме разработки, попадают сюда.

## Типовой сценарий использования

1. **Регистрация.** `POST /api/User/register`. На указанный email отправляется письмо со ссылкой подтверждения (в dev-режиме — в MailHog).
2. **Подтверждение email.** Переход по ссылке из письма активирует аккаунт.
3. **Логин.** `POST /api/User/login-user` возвращает JWT. Фронтенд сохраняет токен и использует его в заголовке `Authorization: Bearer <token>`.
4. **Добавление паспорта.** `POST /api/User/add-passport/{userId}` — обязательно для оформления аренды.
5. **Просмотр каталога.** `GET /api/Car/available` — публичный список доступных авто. `GET /api/Car/detailed-car/{carId}` — подробности.
6. **Оформление аренды.**
   - Создать договор: `POST /api/Contract/create-contract`.
   - Подписать договор: `PUT /api/Contract/sign-contract`.
   - Создать аренду: `POST /api/Rental/CreateRental`.
   - Оплатить (депозит или полная сумма): `POST /api/Payments/pay/{rentalId}?type=Deposit|FullPayment` → редирект на страницу bePaid, после оплаты фронт вернётся на `BEPAID_CALLBACK_URL`.
7. **Возврат авто.** `POST /api/Rental/RequestReturn/{id}` — запрос на возврат. Завершение аренды и расчёт финальной стоимости: `PUT /api/Rental/EndRental/{id}`.
8. **Администрирование.** Раздел `/admin/*` на фронтенде — управление авто (добавление, изменение статуса, отправка на обслуживание/ремонт), пользователями (активация, деактивация, удаление) и шаблонами договоров.

## Структура репозитория

```
CarRentalAgency/
├── Backend/
│   ├── docker-compose.yml
│   ├── .env
│   ├── Gateway/                  # YARP API Gateway
│   ├── Services/
│   │   ├── CarService/
│   │   ├── UserService/
│   │   ├── RentalService/
│   │   ├── ContractService/
│   │   ├── PaymentService/
│   │   ├── NotificationService/
│   │   └── ... (Contracts, Storage, InternetMarket.PaymentService — служебные)
│   ├── Scripts/                  # SQL-скрипты (fix-missing-columns.sql и др.)
│   └── Storage/Contracts/        # том для сгенерированных PDF договоров
└── Frontend/                     # React-приложение
```

## Полезные команды

```bash
# Статус контейнеров
docker compose -f Backend/docker-compose.yml ps

# Перезапуск конкретного сервиса
docker compose -f Backend/docker-compose.yml restart user-service

# Логи всех сервисов
docker compose -f Backend/docker-compose.yml logs -f

# Полная очистка (с удалением данных БД)
docker compose -f Backend/docker-compose.yml down -v

# Линт фронтенда
cd Frontend && npm run lint
```

## Заметки

- Значения в `Backend/.env` предназначены только для локальной разработки. Перед деплоем замените все секреты и храните их в секрет-менеджере (Docker Secrets, Vault и т. п.).
- Тестовый шлюз bePaid подходит только для разработки. Для боевого приёма платежей получите реальные `BEPAID_SHOP_ID` и `BEPAID_SECRET_KEY` и обеспечьте публичную доступность `BEPAID_NOTIFICATION_URL` (например, через ngrok в dev-среде или белый список IP в проде).
- Сгенерированные PDF договоров сохраняются в `Backend/Storage/Contracts/` (монтируется в контейнер `contract-service`).
