[RU](README.md) | [EN](README.en.md)

# Maticon Office DocSpace Server

[![License: AGPLv3](https://img.shields.io/badge/license-AGPLv3-orange.svg)](LICENSE)
[![.NET 7](https://img.shields.io/badge/.NET-7.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![Release](https://img.shields.io/badge/release-2.0.0-146EF5.svg)](https://github.com/MaticonOffice/DocSpace-server)

The server side of Maticon Office DocSpace, a multi-tenant platform for document management, rooms, collaboration, and document editor integration. This snapshot targets release branch `2.0.0` and contains the backend applications, background services, database migrations, and shared libraries.

The complete product is assembled with [MaticonOffice/DocSpace-client](https://github.com/MaticonOffice/DocSpace-client) and [MaticonOffice/DocSpace-buildtools](https://github.com/MaticonOffice/DocSpace-buildtools). The main product entry point is [MaticonOffice/DocSpace](https://github.com/MaticonOffice/DocSpace).

## Features

- portals and rooms with role-based access;
- file storage, upload, sharing, and conversion;
- users, groups, guests, and profile management;
- document server and external storage integrations;
- LDAP/Active Directory, SSO, and federated authentication;
- backup, audit, notifications, and background processing;
- APIs, webhooks, and real-time services;
- MySQL and PostgreSQL migrations for SaaS and standalone deployments.

## Technology stack

- C# and ASP.NET Core on `.NET 7`;
- Entity Framework Core;
- MySQL and PostgreSQL;
- Redis and distributed caching;
- RabbitMQ and ActiveMQ through a shared event bus;
- Elasticsearch for search;
- WebSocket/Socket.IO, SMTP, and external OAuth providers.

The exact infrastructure set depends on the selected configuration and deployment profile.

## Repository layout

```text
common/       shared libraries, integrations, services, and tests
migrations/   MySQL/PostgreSQL migrations for SaaS and standalone
products/     Files and People servers, file services, and tests
web/          API, Studio, shared web logic, and Health Checks UI
thirdparty/   unmodified third-party sources and dependencies
.nuget/       NuGet restore configuration
```

Primary solution files:

- `ASC.Web.sln` — web applications, product services, and shared backend components;
- `ASC.Tests.sln` — server test projects;
- `ASC.Migrations.sln` — migration tools and provider projects;
- `thirdparty.sln` — third-party projects shipped with the source tree.

The `ASC.*` prefix is retained as a compatibility namespace and assembly naming scheme. Product names, domains, repository URLs, and legacy identifiers use the Maticon Office identity.

## Requirements

- Git with submodule support;
- .NET SDK 7.x;
- MySQL or PostgreSQL for the selected profile;
- Redis, a message broker, and a search service when enabled by configuration;
- Node.js for individual JavaScript services under `common/`;
- matching client and buildtools release `2.0.0` for a complete product build.

## Clone the repository

```bash
git clone --recurse-submodules https://github.com/MaticonOffice/DocSpace-server.git
cd DocSpace-server
git submodule update --init --recursive
```

The snapshot pins two submodules:

- `products/ASC.Files/Server/DocStore` — document templates;
- `common/Tests/Frontend.Translations.Tests/dictionaries` — translation test dictionaries.

Their commit SHAs are controlled by the parent repository. Gitlink updates are handled separately from rebranding changes.

## Build

Restore dependencies and build the main backend solution:

```bash
dotnet restore ASC.Web.sln
dotnet build ASC.Web.sln -c Release --no-restore
```

Build the migration tools separately when required:

```bash
dotnet restore ASC.Migrations.sln
dotnet build ASC.Migrations.sln -c Release --no-restore
```

For a complete product build, place server, client, and buildtools in the layout expected by Maticon Office DocSpace Build Tools and run the buildtools entry point for the target platform.

## Configuration and local run

Application configuration is stored next to each entry point, including:

- `web/ASC.Web.Studio/appsettings.json`;
- `web/ASC.Web.Api/appsettings.json`;
- `products/ASC.Files/Server/appsettings.json`;
- `products/ASC.People/Server/appsettings.json`;
- `common/services/*/appsettings.json`.

Before startup, configure database connection strings, Redis, message brokers, file storage, SMTP, and any enabled integrations. Do not commit secrets to the repository.

An individual entry point can be started for local diagnostics with a command such as:

```bash
dotnet run --project web/ASC.Web.Studio/ASC.Web.Studio.csproj
```

Use a compatible buildtools checkout to run the complete service set. It prepares the configuration and starts the backend, client, and infrastructure dependencies together.

## Database migrations

Migration projects are located under `migrations/mysql` and `migrations/postgre`. Each provider includes `SaaS` and `Standalone` profiles. Select the profile that matches the deployment configuration and back up the database before applying migrations.

Migration runner and creator tools are located in `common/Tools/ASC.Migration.Runner`, `common/Tools/ASC.Migration.Creator`, and `common/Tools/ASC.Migrations.Core`.

## Tests

```bash
dotnet restore ASC.Tests.sln
dotnet test ASC.Tests.sln -c Release --no-restore
```

Some integration tests require databases, Redis, message brokers, or other external services. Provide test environment settings through local configuration and environment variables.

## API and support

- API: [api.maticonoffice.ru/docspace](https://api.maticonoffice.ru/docspace/)
- Website: [maticonoffice.ru](https://maticonoffice.ru/)
- Support: [support.maticonoffice.ru](https://support.maticonoffice.ru/)
- Issues: [MaticonOffice/DocSpace-server](https://github.com/MaticonOffice/DocSpace-server/issues)

When opening an issue, include the version, deployment method, database provider, reproduction steps, and relevant logs with secrets and personal data removed.

## Contributing

1. Create a branch from the current release base.
2. Keep the change focused on one component or fix.
3. Add or update tests for changed behavior.
4. Build the relevant solution and run the targeted tests.
5. Do not modify third-party licenses, notices, or original copyright statements.

## License

The code is distributed under the GNU Affero General Public License v3 with the additional terms in [LICENSE](LICENSE). License texts, copyright statements, and third-party notices retain their original legal attribution.
