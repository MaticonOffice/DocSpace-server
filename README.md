# Сервер Матикон Офис Докспейс

[RU](README.md) | [EN](README.en.md)

[![Примечания к выпуску](https://img.shields.io/github/release/MaticonOffice/DocSpace?style=flat-square)](https://github.com/MaticonOffice/DocSpace/releases)
[![Лицензия](https://img.shields.io/badge/license-AGPLv3-orange)](https://opensource.org/license/agpl-v3)
[![Звёзды GitHub](https://img.shields.io/github/stars/MaticonOffice/DocSpace?style=flat-square)](https://star-history.com/#MaticonOffice/DocSpace)
[![Открытые задачи](https://img.shields.io/github/issues-raw/MaticonOffice/DocSpace?style=flat-square)](https://github.com/MaticonOffice/DocSpace/issues)

Этот репозиторий содержит **серверную часть** [Матикон Офис Докспейс](https://github.com/MaticonOffice/DocSpace), платформы для совместной работы и управления документами на основе комнат.

> Полное описание продукта приведено в [README основного репозитория](https://github.com/MaticonOffice/DocSpace#readme).
> Настройка и архитектура клиентской части описаны в [README клиента](https://github.com/MaticonOffice/DocSpace-client#readme).

## Содержание

- [Технологический стек](#технологический-стек)
- [Структура проекта](#структура-проекта)
- [Начало работы](#начало-работы)
  - [Предварительные требования](#предварительные-требования)
  - [Быстрый запуск](#быстрый-запуск)
  - [HTTPS и многопортальная среда разработки](#https-и-многопортальная-среда-разработки)
  - [Профили запуска](#профили-запуска)
  - [Разработка в VS Code](#разработка-в-vs-code)
  - [Очистка Docker-артефактов Aspire](#очистка-docker-артефактов-aspire)
- [Распределение портов](#распределение-портов)
- [Тестирование](#тестирование)
  - [Миграции базы данных](#миграции-базы-данных)
- [Устранение неполадок](#устранение-неполадок)
- [Участие в разработке](#участие-в-разработке)
- [Лицензирование](#лицензирование)

## Технологический стек

### Основа

- **Язык:** C# 14.0
- **Среда выполнения:** .NET 10.0 и ASP.NET Core
- **Оркестрация:** пакеты .NET Aspire 13.2/13.3
- **Контейнер внедрения зависимостей:** Autofac 10.0
- **Версионирование API:** Asp.Versioning 8.1
- **Сопоставление объектов:** Riok.Mapperly 4.3

### Данные и хранилища

- **База данных:** MySQL 9.5 как основной вариант, также поддерживается PostgreSQL
- **Кэширование:** Redis, StackExchange.Redis 2.10 и FusionCache 2.5
- **Поиск:** OpenSearch
- **Хранилище:** абстрактный слой с несколькими поставщиками

### Обмен сообщениями и связь

- **Брокеры сообщений:** RabbitMQ 7.2 как основной вариант, а также Apache Kafka, ActiveMQ и RedisMQ
- **WebSocket:** Socket.IO для обновлений в реальном времени
- **Вебхуки:** встроенная поддержка вебхуков

### Аутентификация и безопасность

- **Аутентификация:** JWT Bearer и OpenID Connect
- **Федерация:** SAML SSO, Active Directory и LDAP
- **Безопасность:** фильтрация IP, защита от перебора, двухфакторная аутентификация и ограничение частоты запросов

### Наблюдаемость

- **Журналирование:** NLog 5.5 с целевыми системами Elasticsearch, Syslog и AWS CloudWatch
- **Трассировка:** OpenTelemetry 1.15 с экспортом OTLP
- **Проверки работоспособности:** ASP.NET Health Checks UI для всех сервисов

### Документация API

- **Swagger:** Swashbuckle 10.1
- **Интерактивная документация:** Scalar 2.12

### Искусственный интеллект

- **Интеграция ИИ:** Mscc.GenerativeAI.Microsoft 2.9

### Инфраструктура

- **Контейнеризация:** Docker 28.5 или новее
- **Обратный прокси:** OpenResty на основе Nginx

## Структура проекта

Проект организован как **решение .NET** с микросервисной архитектурой, несколькими сервисами и общими библиотеками.

### Обзор решения

```text
DocSpace-server/
|-- common/                         # Общие библиотеки и сервисы
|   |-- ASC.AppHost/                # Оркестратор .NET Aspire
|   |-- ASC.Api.Core/               # Основа API
|   |-- ASC.Core.Common/            # Основная бизнес-логика
|   |-- ASC.Common/                 # Общие служебные средства
|   |-- ASC.Data.Storage/           # Абстракция хранилища
|   |-- ASC.Data.Backup.Core/       # Основа резервного копирования
|   |-- ASC.Data.Encryption/        # Шифрование данных
|   |-- ASC.Data.Reassigns/         # Переназначение пользовательских данных
|   |-- ASC.EventBus/               # RabbitMQ, ActiveMQ, Redis и Kafka
|   |-- ASC.FederatedLogin/         # Федерация и единый вход
|   |-- ASC.Identity/               # Управление удостоверениями
|   |-- ASC.ActiveDirectory/        # Интеграция Active Directory
|   |-- ASC.IPSecurity/             # Безопасность по IP
|   |-- ASC.MessagingSystem/        # Внутренние сообщения
|   |-- ASC.Migration/              # Основа миграции
|   |-- ASC.Resource.Manager/       # Управление ресурсами
|   |-- ASC.Socket.IO/              # Поддержка WebSocket
|   |-- ASC.SsoAuth/                # Аутентификация SSO
|   |-- ASC.Thumbnails/             # Создание миниатюр
|   |-- ASC.WebDav/                 # Поддержка WebDAV
|   |-- ASC.Webhooks.Core/          # Поддержка вебхуков
|   |-- ASC.Analyzers/              # Пользовательские анализаторы кода
|   |-- services/                   # Фоновые сервисы
|   |   |-- ASC.Notify/             # Сервис уведомлений
|   |   |-- ASC.Studio.Notify/      # Уведомления Studio
|   |   |-- ASC.Data.Backup/        # Сервис резервного копирования
|   |   |-- ASC.Data.Backup.Worker/ # Обработчик резервного копирования
|   |   |-- ASC.ElasticSearch/      # Поисковая инфраструктура
|   |   |-- ASC.ApiSystem/          # Системные сервисы API
|   |   |-- ASC.TelegramService/    # Интеграция с Telegram
|   |   |-- ASC.AuditTrail/         # Журнал аудита
|   |   `-- ASC.ClearEvents/        # Очистка событий
|   `-- Tools/                      # Инструменты разработки
|       |-- ASC.Migration.Runner/   # Запуск миграций БД
|       |-- ASC.Migrations.Core/    # Среда миграций
|       |-- ASC.Api.Documentation/  # Генератор документации API
|       `-- ASC.Data.Stress/        # Нагрузочное тестирование
|-- products/                       # Основные модули продукта
|   |-- ASC.Files/                  # Управление документами
|   |   |-- Server/                 # Files API, порт 5007
|   |   |-- Worker/                 # Обработчик Files, порт 5009
|   |   |-- Core/                   # Бизнес-логика Files
|   |   `-- Tests/                  # Тесты Files
|   |-- ASC.People/                 # Управление пользователями
|   |   |-- Server/                 # People API, порт 5004
|   |   `-- Tests/                  # Тесты People
|   `-- ASC.AI/                     # Функции ИИ
|       |-- Server/                 # AI API, порт 5157
|       |-- Worker/                 # Обработчик ИИ, порт 5154
|       `-- Core/                   # Бизнес-логика ИИ
|-- web/                            # Веб-уровень
|   |-- ASC.Web.Api/                # Основной REST API, порт 5000
|   |-- ASC.Web.Studio/             # Сервер Studio, порт 5003
|   |-- ASC.Web.Core/               # Общая веб-основа
|   `-- ASC.Web.HealthChecks.UI/    # Мониторинг работоспособности
|-- migrations/                     # Миграции базы данных
|   |-- mysql/
|   |   |-- SaaS/
|   |   `-- Standalone/
|   `-- postgre/
|       |-- SaaS/
|       `-- Standalone/
|-- sdk/                            # Подмодули SDK API
|   |-- docspace-api-sdk-python/
|   |-- docspace-api-sdk-java/
|   |-- docspace-api-sdk-kotlin/
|   |-- docspace-api-sdk-swift/
|   |-- docspace-api-sdk-php/
|   |-- docspace-api-sdk-typescript/
|   |-- docspace-api-sdk-csharp/
|   `-- docspace-api-postman-collections/
|-- thirdparty/                     # Сторонние библиотеки
|-- ASC.Web.sln                     # Основное решение
|-- ASC.Tests.slnx                  # Решение с тестами
|-- ASC.Migrations.sln             # Решение миграций
`-- Directory.Packages.props        # Централизованные версии NuGet
```

### Назначение сервисов

#### ASC.Web.Api - основной REST API

Центральный шлюз API для файлов, пользователей, комнат, настроек и аутентификации DocSpace.
**Порт:** 5000

#### ASC.Web.Studio - сервер Studio

Серверная часть веб-интерфейса DocSpace: управление порталом, фирменное оформление и управление плагинами.
**Порт:** 5003

#### ASC.Files - управление документами

Основная логика документов: файловые хранилища, сторонние облачные интеграции, преобразование документов, общий доступ, разрешения и фоновая обработка.
**Порты:** 5007 для сервера и 5009 для обработчика.

#### ASC.People - управление пользователями

Управление пользователями и группами: профили, настройки, импорт и экспорт, переназначение данных.
**Порт:** 5004

#### ASC.AI - функции искусственного интеллекта

Интеграция помощника ИИ, фоновая обработка и поддержка генеративного ИИ.
**Порты:** 5157 для сервера и 5154 для обработчика.

#### ASC.AppHost - оркестратор Aspire

Хост .NET Aspire управляет всеми сервисами и инфраструктурой: обнаружением сервисов, контейнерами Docker с MySQL, Redis, RabbitMQ и OpenSearch, а также инструментами MailPit, DBGate и RedisInsight.

### Шина событий

Сервер поддерживает несколько реализаций брокера сообщений:

- **RabbitMQ** - основной брокер;
- **ActiveMQ** - альтернативный брокер;
- **RedisMQ** - обмен сообщениями через Redis;
- **Kafka** - высокопроизводительная потоковая обработка.

### SDK API

Официальные SDK API подключены как подмодули для нескольких языков:

- Python, Java, Kotlin, Swift, PHP, TypeScript и C#;
- коллекции Postman для исследования API.

## Начало работы

> **Примечание:** это среда **разработки и тестирования**, не предназначенная для рабочего развёртывания.
> Варианты рабочего развёртывания приведены на странице [загрузок Матикон Офис Докспейс](https://www.maticonoffice.ru/download#docspace-enterprise).

### Предварительные требования

| Инструмент | Версия | Команда проверки |
|------------|--------|------------------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0 | `dotnet --version` |
| [Docker](https://www.docker.com/) | 28.5.0 или новее | `docker --version` |

### Быстрый запуск

```bash
# Из корня репозитория DocSpace-server
cd common/ASC.AppHost
dotnet run --launch-profile development
```

**Адреса:**

- панель Aspire: http://localhost:15208
- документация API Scalar: http://localhost:8092/scalar/#ascfiles
- DBGate: http://localhost:56161
- Mailpit: http://localhost:56162

> Запуск полного приложения с клиентской частью описан в [README клиента](https://github.com/MaticonOffice/DocSpace-client#readme).

### HTTPS и многопортальная среда разработки

Помимо обычного HTTP на `http://localhost:8092`, AppHost публикует тот же набор сервисов через HTTPS на `https://docspace.dev.localhost`, порт `443`. Это обеспечивает аутентификацию через cookie, защищённые WebSocket и разработку с несколькими порталами, где каждый клиент работает на собственном поддомене вида `https://<alias>.dev.localhost`.

При запуске Aspire конфигурация создаётся автоматически. Изменять файл `hosts` и выполнять команды администратора не требуется:

| Задача | Реализация |
|--------|------------|
| **DNS** | Суффикс `.dev.localhost` разрешается в `127.0.0.1` по RFC 6761 в браузерах. Для SSR login/doceditor/management/sdk через `NODE_OPTIONS=--require=...` подключается `ASC.AppHost/scripts/docspace-dns-patch.js`. |
| **Сертификат** | При первом запуске `DevCertificateGenerator` создаёт в `ASC.AppHost/certs/` самоподписанный сертификат с SAN `localhost` и `*.dev.localhost`. Он действует два года и перевыпускается при истечении срока или изменении SAN. |
| **Доверие** | В Windows сертификат автоматически устанавливается в доверенные корневые центры текущего пользователя. В macOS он добавляется в связку ключей входа после одного запроса пароля. В Linux AppHost выводит команды `update-ca-certificates` или `certutil`, которые нужно выполнить вручную. |
| **Завершение TLS** | Контейнер OpenResty слушает порт `443`, передаёт запросы на внутренний HTTP `127.0.0.1:8092` и сохраняет заголовки `X-Forwarded-Proto/Host/For/Ssl`. |
| **Переадресованные заголовки** | Все серверные сервисы доверяют loopback и сетям Docker bridge через `core:hosting:forwardedHeadersOptions:knownNetworks`, поэтому `Request.Url()` видит исходные схему `https://` и имя узла. |
| **Определение клиента** | В автономном режиме `core:base-domain` получает значение `dev.localhost`: узел `nct.dev.localhost` соответствует псевдониму клиента `nct`. Стандартный автономный клиент сопоставляется с `docspace.dev.localhost` переменной `CORE__LOCAL_ADDRESSES`. |
| **Источники Next.js** | `getAllLocalIps` из `client/packages/shared/utils/build.js` добавляет `localhost` и `*.dev.localhost` в `allowedDevOrigins`, чтобы серверы разработки принимали запросы с поддоменов порталов. |

**Использование:**

- стандартный портал: `https://docspace.dev.localhost`;
- создайте в интерфейсе управления портал с псевдонимом `foo`, затем откройте `https://foo.dev.localhost`;
- параллельно продолжает работать HTTP на `http://localhost:8092`.

**Сброс сертификата** после изменения SAN или рассинхронизации отметки доверия:

1. Удалите из `common/ASC.AppHost/certs/` файлы `.crt`, `.key` и `.trusted`.
2. В Windows также удалите устаревшие сертификаты `localhost` или `docspace.dev.localhost` через `certmgr` из доверенных корневых центров сертификации текущего пользователя.
3. Перезапустите Aspire: будет создан и добавлен в доверенные новый сертификат.

### Профили запуска

Перейдите в `common/ASC.AppHost` и выберите профиль:

| Профиль | Команда | Описание |
|---------|---------|----------|
| `development` | `dotnet run --launch-profile development` | Полная среда разработки со всеми сервисами |
| `frontend-dev` | `dotnet run --launch-profile frontend-dev` | Все серверные сервисы без сборки клиента, который запускается отдельно |
| `preview` | `dotnet run --launch-profile preview` | Минимальная среда на основе Docker |

> Aspire запускает несколько сервисов: часть напрямую, часть в контейнерах Docker. Нажмите `Ctrl+C`, чтобы остановить все сервисы.

### Разработка в VS Code

**1. Установите рекомендуемые расширения:**

- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) для обозревателя решений, IntelliSense и рефакторинга;
- [.NET Aspire](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-aspire) для запуска и отладки оркестрации Aspire.

**2. Откройте решение:**

```bash
code .
```

C# Dev Kit автоматически обнаружит `ASC.Web.sln`. Для ускорения загрузки можно открыть подходящее решение:

- `ASC.Web.sln` - полное решение со всеми проектами;
- `ASC.Web.slnx` - XML-формат решения;
- `ASC.Tests.slnx` - только тестовые проекты;
- `ASC.Migrations.sln` - только проекты миграций.

**3. Запустите отладку:**

С расширением .NET Aspire проект `ASC.AppHost` можно запустить из VS Code клавишей `F5` или через панель «Запуск и отладка» (`Ctrl+Shift+D`). Расширение организует запуск сервисов и автоматически откроет панель Aspire.

Чтобы отладить отдельный сервис, установите точки останова и подключитесь к выполняющемуся процессу через панель Aspire или отладчик VS Code.

### Очистка Docker-артефактов Aspire

Linux и macOS, Bash:

```bash
docker ps -a --format '{{.Names}}' | grep -E 'mysql|redis|cache-|rabbitmq|messaging-|opensearch|mailpit|dbgate|redisinsight|maticonoffice-editors|openresty' | xargs -r docker stop && \
docker ps -a --format '{{.Names}}' | grep -E 'mysql|redis|cache-|rabbitmq|messaging-|opensearch|mailpit|dbgate|redisinsight|maticonoffice-editors|openresty' | xargs -r docker rm && \
docker volume prune -f && docker network prune -f
```

Windows, PowerShell:

```powershell
$c = docker ps -a --format '{{.Names}}' | Where-Object { $_ -match 'mysql|redis|cache-|rabbitmq|messaging-|opensearch|mailpit|dbgate|redisinsight|maticonoffice-editors|openresty' }; if ($c) { $c | ForEach-Object { docker stop $_ }; $c | ForEach-Object { docker rm $_ } }; docker volume prune -f; docker network prune -f
```

## Распределение портов

| Сервис | Порт | Назначение |
|--------|------|------------|
| OpenResty, обратный прокси | 8092 | Шлюз API |
| Панель Aspire | 15208 | Мониторинг серверных сервисов |
| DBGate | 56161 | Интерфейс управления базой данных |
| Mailpit | 56162 | Тестирование электронной почты |
| Web API | 5000 | Основной REST API |
| Web Studio | 5003 | Сервер Studio |
| People | 5004 | Управление пользователями |
| Notify | 5005 | Сервис уведомлений |
| Studio Notify | 5006 | Уведомления Studio |
| Files | 5007 | Управление документами |
| Files Worker | 5009 | Обработка файлов |
| API System | 5010 | Системные API |
| Backup | 5012 | Резервное копирование |
| Clear Events | 5027 | Очистка событий |
| Backup Worker | 5032 | Обработчик резервного копирования |
| Telegram | 5050 | Интеграция с Telegram |
| AI | 5157 | Сервис ИИ |
| AI Worker | 5154 | Обработка ИИ |
| Identity Authorization | 8080 | Сервис авторизации |
| Identity Registration | 9090 | Сервис удостоверений |
| Socket.IO | 9899 | WebSocket в реальном времени |
| SSO Auth | 9834 | Аутентификация SSO |
| WebDAV | 1900 | Протокол WebDAV |
| MySQL | 3306 | Сервер базы данных |
| Redis | 6379 | Сервер кэша |
| RabbitMQ | 5672, 15672 | Брокер и интерфейс управления |
| OpenSearch | 9200, 9600 | Поисковая система |

## Тестирование

Решение `ASC.Tests.slnx` содержит модульные и интеграционные тесты серверных сервисов.

```bash
# Все тесты
dotnet test ASC.Tests.slnx

# Тесты конкретного проекта
dotnet test products/ASC.Files/Tests/ASC.Files.Tests.csproj

# Тесты People
dotnet test products/ASC.People/Tests/ASC.People.Tests.csproj
```

### Миграции базы данных

Миграции организованы отдельно для каждого движка базы данных и типа развёртывания:

```bash
# Автономные миграции MySQL
cd common/Tools/ASC.Migration.Runner
dotnet run
```

## Устранение неполадок

<details>
<summary><b>Порт 8092 уже используется</b></summary>

Завершите процесс, занимающий порт:

```bash
# macOS/Linux
lsof -ti:8092 | xargs kill -9

# Windows
netstat -ano | findstr :8092
taskkill /PID <PID> /F
```
</details>

<details>
<summary><b>Контейнеры Docker не запускаются</b></summary>

1. Убедитесь, что Docker работает: `docker ps`.
2. Очистите артефакты Docker, как описано в разделе [«Очистка Docker-артефактов Aspire»](#очистка-docker-артефактов-aspire).
3. Перезапустите Docker Desktop.
4. Снова запустите серверную часть.
</details>

<details>
<summary><b>dotnet run сообщает об ошибке версии SDK</b></summary>

Убедитесь, что установлен .NET SDK 10.0:

```bash
dotnet --list-sdks
```
</details>

<details>
<summary><b>Серверные сервисы не останавливаются</b></summary>

Принудительно остановите все контейнеры Docker:

```bash
docker stop $(docker ps -aq) && docker rm $(docker ps -aq)
```
</details>

Другие проблемы можно зарегистрировать в [системе задач](https://github.com/MaticonOffice/DocSpace/issues) или обсудить на [форуме](https://forum.maticonoffice.ru/).

## Участие в разработке

### Рабочий процесс

1. Создайте форк репозитория.
2. Клонируйте форк: `git clone https://github.com/YOUR_USERNAME/DocSpace.git`.
3. Создайте ветку: `git checkout -b feature/amazing-feature`.
4. Внесите изменения.
5. Запустите тесты: `dotnet test`.
6. Создайте коммит: `git commit -m 'Add amazing feature'`.
7. Отправьте ветку: `git push origin feature/amazing-feature`.
8. Откройте Pull Request.

### Стандарты кода

- Следуйте рекомендациям C# и .NET.
- Выполняйте `dotnet build`, чтобы исключить ошибки компиляции.
- Добавляйте тесты для новой функциональности.
- Делайте коммиты атомарными и понятными.
- Используйте централизованные версии пакетов из `Directory.Packages.props`.

### Соглашение о сообщениях коммитов

Следуйте [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` - новая возможность;
- `fix:` - исправление ошибки;
- `docs:` - изменение документации;
- `style:` - изменение стиля кода;
- `refactor:` - рефакторинг;
- `test:` - изменение тестов;
- `chore:` - сборка или инструменты.

## Лицензирование

Матикон Офис Докспейс распространяется по лицензии AGPLv3. Дополнительные сведения приведены в файле LICENSE.

## Помощь разработчикам

Используйте [официальную документацию API](https://api.maticonoffice.ru/docspace/).
