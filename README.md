[RU](README.md) | [EN](README.en.md)

# Сервер Матикон Офис Докспейс

[![License: AGPLv3](https://img.shields.io/badge/license-AGPLv3-orange.svg)](LICENSE)
[![.NET 7](https://img.shields.io/badge/.NET-7.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![Release](https://img.shields.io/badge/release-2.0.0-146EF5.svg)](https://github.com/MaticonOffice/DocSpace-server)

Серверная часть Матикон Офис Докспейс: многопользовательской платформы для управления документами, комнатами, совместной работы и интеграции редакторов. Этот snapshot подготовлен для ветки релиза `2.0.0` и содержит backend, фоновые сервисы, миграции баз данных и общие библиотеки.

Полное приложение собирается вместе с [MaticonOffice/DocSpace-client](https://github.com/MaticonOffice/DocSpace-client) и [MaticonOffice/DocSpace-buildtools](https://github.com/MaticonOffice/DocSpace-buildtools). Общая точка входа продукта находится в репозитории [MaticonOffice/DocSpace](https://github.com/MaticonOffice/DocSpace).

## Возможности

- порталы и комнаты с разграничением доступа;
- хранение, загрузка, совместное использование и конвертация файлов;
- пользователи, группы, гости и профили;
- интеграция серверов документов и внешних хранилищ;
- LDAP/Active Directory, SSO и федеративная аутентификация;
- резервное копирование, аудит, уведомления и фоновые задачи;
- API, webhooks и сервисы реального времени;
- миграции MySQL и PostgreSQL для SaaS и standalone-развёртываний.

## Технологии

- C# и ASP.NET Core на `.NET 7`;
- Entity Framework Core;
- MySQL и PostgreSQL;
- Redis и распределённое кэширование;
- RabbitMQ и ActiveMQ через общий event bus;
- Elasticsearch для поиска;
- WebSocket/Socket.IO, SMTP и внешние OAuth-провайдеры.

Конкретный набор инфраструктурных сервисов определяется выбранной конфигурацией и сценарием развёртывания.

## Структура репозитория

```text
common/       общие библиотеки, интеграции, сервисы и тесты
migrations/   миграции MySQL/PostgreSQL для SaaS и standalone
products/     серверы Files и People, файловые сервисы и тесты
web/          API, Studio, общая web-логика и Health Checks UI
thirdparty/   неизменённые сторонние исходники и зависимости
.nuget/       конфигурация восстановления NuGet
```

Основные solution-файлы:

- `ASC.Web.sln` — web-приложения, продуктовые сервисы и общие backend-компоненты;
- `ASC.Tests.sln` — серверные тестовые проекты;
- `ASC.Migrations.sln` — инструменты и проекты миграций;
- `thirdparty.sln` — сторонние проекты, поставляемые с исходным деревом.

Префикс `ASC.*` сохранён как совместимый внутренний namespace и имя сборок. Продуктовые названия, домены, URL репозиториев и устаревшие идентификаторы переведены на Maticon Office.

## Требования

- Git с поддержкой submodule;
- .NET SDK 7.x;
- MySQL или PostgreSQL для выбранного профиля;
- Redis, брокер сообщений и поисковый сервис, если они включены конфигурацией;
- Node.js для отдельных JavaScript-сервисов в `common/`;
- совместимые client и buildtools release `2.0.0` для полной сборки продукта.

## Получение исходного кода

```bash
git clone --recurse-submodules https://github.com/MaticonOffice/DocSpace-server.git
cd DocSpace-server
git submodule update --init --recursive
```

В snapshot закреплены два submodule:

- `products/ASC.Files/Server/DocStore` — шаблоны документов;
- `common/Tests/Frontend.Translations.Tests/dictionaries` — словари тестов переводов.

Их commit SHA задаются родительским репозиторием. Обновление gitlink выполняется отдельно от ребрендинга.

## Сборка

Восстановите зависимости и соберите основной backend:

```bash
dotnet restore ASC.Web.sln
dotnet build ASC.Web.sln -c Release --no-restore
```

Миграции можно собрать отдельно:

```bash
dotnet restore ASC.Migrations.sln
dotnet build ASC.Migrations.sln -c Release --no-restore
```

Для полной продуктовой сборки разместите server, client и buildtools в структуре, ожидаемой скриптами Матикон Офис Докспейс Build Tools, и запускайте buildtools выбранной платформы.

## Конфигурация и запуск

Конфигурация приложений хранится рядом с соответствующими entry point, например:

- `web/ASC.Web.Studio/appsettings.json`;
- `web/ASC.Web.Api/appsettings.json`;
- `products/ASC.Files/Server/appsettings.json`;
- `products/ASC.People/Server/appsettings.json`;
- `common/services/*/appsettings.json`.

Перед запуском задайте строки подключения, адреса Redis, брокера сообщений, файлового хранилища, SMTP и других включённых интеграций. Не фиксируйте секреты в репозитории.

Для локальной диагностики отдельный entry point можно запустить командой вида:

```bash
dotnet run --project web/ASC.Web.Studio/ASC.Web.Studio.csproj
```

Полный набор сервисов рекомендуется запускать через совместимый checkout buildtools: он формирует конфигурацию и согласованно поднимает backend, client и инфраструктурные зависимости.

## Миграции базы данных

Проекты миграций находятся в `migrations/mysql` и `migrations/postgre`. В каждом провайдере есть профили `SaaS` и `Standalone`. Используйте профиль, соответствующий конфигурации развёртывания, и создавайте резервную копию базы перед применением миграций.

Инструменты запуска и создания миграций находятся в `common/Tools/ASC.Migration.Runner`, `common/Tools/ASC.Migration.Creator` и `common/Tools/ASC.Migrations.Core`.

## Тесты

```bash
dotnet restore ASC.Tests.sln
dotnet test ASC.Tests.sln -c Release --no-restore
```

Некоторые интеграционные тесты требуют доступных баз данных, Redis, брокеров или других внешних сервисов. Параметры тестовой среды задавайте через локальную конфигурацию и переменные окружения.

## API и поддержка

- API: [api.maticonoffice.ru/docspace](https://api.maticonoffice.ru/docspace/)
- Сайт: [maticonoffice.ru](https://maticonoffice.ru/)
- Поддержка: [support.maticonoffice.ru](https://support.maticonoffice.ru/)
- Issues: [MaticonOffice/DocSpace-server](https://github.com/MaticonOffice/DocSpace-server/issues)

При создании issue укажите версию, способ развёртывания, используемую СУБД, шаги воспроизведения и релевантные логи без секретов и персональных данных.

## Участие в разработке

1. Создайте ветку от актуальной release-базы.
2. Ограничьте изменение одним компонентом или исправлением.
3. Добавьте или обновите тесты для изменённого поведения.
4. Выполните сборку соответствующего solution и целевые тесты.
5. Не меняйте сторонние лицензии, уведомления и исходные copyright-заявления.

## Лицензия

Код распространяется на условиях GNU Affero General Public License v3 с дополнительными условиями из файла [LICENSE](LICENSE). Тексты лицензий, copyright-заявления и third-party notices сохраняют исходную юридическую атрибуцию.
