# InvenireServer (Backend)

Tento repozitář obsahuje serverovou část aplikace zodpovědnou za autentizaci, autorizaci, business logiku a persistenci.

- hlavní dokumentační rozcestník projektu je v repozitáři frontendu [Invenire](https://github.com/realtobi999/Invenire)
- oficiální technická práce v repozitáři frontendu

## Obsah

- [Rozsah projektu](#rozsah-projektu)
- [Technologický stack a runtime požadavky](#technologický-stack-a-runtime-požadavky)
- [Rychlý start](#rychlý-start)
- [Přehled promměných prostředí](#Přehled-promměných-prostředí)
- [Přehled API](#přehled-api)
- [Nastavení databáze a migrace](#nastavení-databáze-a-migrace)
- [Struktura složek](#struktura-složek)
- [Testování](#testování)
- [Licence](#licence)

## Rozsah projektu

InvenireServer poskytuje:

- REST API endpointy pod `/api/*`
- autentizaci založenou na JWT (role + policy pro ověřené/neověřené uživatele)
- workflow organizací a pozvánek
- workflow majetku, položek, návrhů a inventur
- background cleanup služby pro neaktuální data
- persistenci do PostgreSQL přes EF Core

## Technologický stack a runtime požadavky

| Oblast | Technologie | Popis |
|---|---|---|
| Runtime | .NET 9 | Moderní .NET runtime pro backend aplikaci s vysokým výkonem a podporou nejnovějších funkcí jazyka C#. |
| Web framework | ASP.NET Core Web API | REST API framework pro tvorbu škálovatelných backendových služeb. |
| Patterny | CQRS + MediatR + FluentValidation | Oddělení čtení a zápisu dat, zpracování požadavků přes mediátor a centralizovaná validace vstupů. |
| ORM | EF Core 9 + Npgsql provider | Objektově-relační mapování pro práci s PostgreSQL databází pomocí entit. |
| Databáze | PostgreSQL 15 | Relační databáze pro ukládání aplikačních dat s podporou pokročilých funkcí a vysoké spolehlivosti. |
| Auth | JWT (cookie nebo Bearer token) | Autentizace uživatelů pomocí JSON Web Tokenů uložených v cookies nebo Authorization hlavičce. |
| Logování | Serilog | Strukturované logování s možností filtrování a ukládání do různých výstupů. |
| Kontejnerizace | Docker + Docker Compose | Izolované prostředí pro běh aplikace a databáze s jednoduchým spuštěním vývojového prostředí. |
| Testy | xUnit + FluentAssertions + Moq + ASP.NET integrační testování | Jednotkové a integrační testy pro ověření funkčnosti aplikace. |

### Runtime požadavky

- .NET SDK 9.0+
- Docker a Docker Compose (doporučeno pro lokální setup)
- PostgreSQL (pro manuální non-Docker běh)

## Rychlý start

### 🚀 Spuštění přes Docker

1. Vytvořte `backend/.env` a vyplňte požadované hodnoty.
2. Spusťte služby:

```bash
cd backend
docker compose -f docker-compose.dev.yml up --build
```

Ve výchozím nastavení (podle env hodnot) běží stack jako:

- API: `http://127.0.0.1:5071`
- PostgreSQL: host `127.0.0.1`, mapovaný port `5433`, kontejnerový port `5432`

### 🛠️ Manuální spuštění (`dotnet run`)

V development režimu se citlivé hodnoty načítají přes **User Secrets**.

1. Inicializace User Secrets:

```bash
dotnet user-secrets init --project src/InvenireServer.Presentation/InvenireServer.Presentation.csproj
```

2. Spuštění API:

```bash
dotnet run --project src/InvenireServer.Presentation/InvenireServer.Presentation.csproj
```

Volitelné pomocné příkazy přes Makefile:

```bash
make run
make watch
```

## Přehled promměných prostředí

| Proměnná | Povinná | Použití | Popis | Příklad |
|---|---|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Ano | ASP.NET host | Běhové prostředí (`Development`, `Production`) | `Development` |
| `API_PORT` | Ano (Docker) | `docker-compose.dev.yml` | Mapování API host/container portu | `5071` |
| `POSTGRES_HOST_PORT` | Ano (Docker) | `docker-compose.dev.yml` | PostgreSQL port vystavený na hostu | `5433` |
| `POSTGRES_PORT` | Ano (Docker) | `docker-compose.dev.yml` | PostgreSQL port uvnitř kontejneru | `5432` |
| `POSTGRES_USER` | Ano (Docker) | Postgres image | DB uživatelské jméno | `invenire` |
| `POSTGRES_PASSWORD` | Ano (Docker) | Postgres image | DB heslo | `change_me` |
| `POSTGRES_DB` | Ano (Docker) | Postgres image | Název DB | `InvenireServerDatabase` |
| `ConnectionStrings__Connection` | Ano | EF Core (`GetConnectionString("Connection")`) | Connection string API databáze | `Host=postgres;Port=5432;...` |
| `Jwt__Issuer` | Ano | JWT validace/generování | JWT issuer/audience | `Invenire` |
| `Jwt__SigningKey` | Ano | JWT validace/generování | JWT signing secret | `long_random_secret` |
| `Jwt__ExpirationTime` | Ano | JWT generování | Expirace tokenu (minuty) | `3000000` |
| `SMTP__Host` | Ano | Odesílání e-mailu | SMTP host | `smtp.ethereal.email` |
| `SMTP__Port` | Ano | Odesílání e-mailu | SMTP port | `587` |
| `SMTP__Username` | Ano | Odesílání e-mailu | SMTP uživatel | `change_me` |
| `SMTP__Password` | Ano | Odesílání e-mailu | SMTP heslo | `change_me` |
| `Frontend__BaseAddress` | Ano | Generování e-mailových odkazů | Base URL frontendu pro verifikaci/obnovu hesla | `http://127.0.0.1:5170` |
| `CORS__AllowedOrigins__0` | Ano | CORS policy | Povolený frontend origin #1 | `http://127.0.0.1:5170` |
| `CORS__AllowedOrigins__1` | Doporučeno | CORS policy | Povolený frontend origin #2 | `http://localhost:5170` |

> Širší konfigurační kontext najdete v repozitáři [Invenire]([Invenire](https://github.com/realtobi999/Invenire)) v `doc/config.md`.

## Přehled API

Přenos autentizace:

- backend přijímá JWT z cookie `JWT` nebo z `Authorization: Bearer <token>`
- většina endpointů je chráněna rolemi/policies

### Server a auth utility endpointy

| Metoda | Cesta | Popis |
|---|---|---|
| `POST` | `/api/logout` | Smaže JWT cookie |
| `GET` | `/api/server/health-check` | Endpoint pro health check |
| `GET` | `/api/server/auth-check` | Ověří autentizovanou session |
| `GET` | `/api/server/auth/role` | Vrátí roli přihlášeného uživatele |

### Účty admin/employee

| Metoda | Cesta | Popis |
|---|---|---|
| `POST` | `/api/admins/register` | Registrace admina |
| `POST` | `/api/admins/login` | Přihlášení admina |
| `POST` | `/api/admins/email-verification/send` | Odeslání verifikačního e-mailu adminovi |
| `POST` | `/api/admins/email-verification/confirm` | Potvrzení verifikace admina |
| `POST` | `/api/admins/password-recovery/send` | Odeslání e-mailu pro obnovu hesla admina |
| `POST` | `/api/admins/password-recovery/recover` | Obnova hesla admina |
| `PUT` | `/api/admins` | Aktualizace aktuálního admina |
| `DELETE` | `/api/admins` | Smazání aktuálního admina |
| `GET` | `/api/admins/profile` | Profil aktuálního admina |
| `POST` | `/api/employees/register` | Registrace zaměstnance |
| `POST` | `/api/employees/login` | Přihlášení zaměstnance (rate-limited) |
| `POST` | `/api/employees/email-verification/send` | Odeslání verifikačního e-mailu zaměstnanci |
| `POST` | `/api/employees/email-verification/confirm` | Potvrzení verifikace zaměstnance |
| `POST` | `/api/employees/password-recovery/send` | Odeslání e-mailu pro obnovu hesla zaměstnance |
| `POST` | `/api/employees/password-recovery/recover` | Obnova hesla zaměstnance |
| `PUT` | `/api/employees` | Aktualizace aktuálního zaměstnance |
| `DELETE` | `/api/employees` | Smazání aktuálního zaměstnance |
| `GET` | `/api/employees/profile` | Profil aktuálního zaměstnance |
| `GET` | `/api/employees/invitations` | Seznam pozvánek aktuálního zaměstnance |

### Organizace a pozvánky

| Metoda | Cesta | Popis |
|---|---|---|
| `GET` | `/api/organizations` | Aktuální organizace (podle role) |
| `GET` | `/api/organizations/{organizationId}` | Organizace podle ID |
| `POST` | `/api/organizations` | Vytvoření organizace |
| `PUT` | `/api/organizations` | Aktualizace organizace |
| `DELETE` | `/api/organizations` | Smazání organizace |
| `POST` | `/api/organizations/invitations` | Vytvoření pozvánky |
| `POST` | `/api/organization/invitations/json-file` | Import pozvánek z JSON |
| `POST` | `/api/organization/invitations/csv-file` | Import pozvánek z CSV |
| `GET` | `/api/organizations/invitations/{invitationId}` | Pozvánka podle ID |
| `PUT` | `/api/organizations/invitations/{invitationId}` | Aktualizace pozvánky |
| `DELETE` | `/api/organizations/invitations/{invitationId}` | Smazání pozvánky |
| `PUT` | `/api/organizations/invitations/{invitationId}/accept` | Přijetí pozvánky |
| `GET` | `/api/organizations/employees/{employeeId}` | Zaměstnanec podle ID |
| `GET` | `/api/organizations/employees/{employeeAddress}` | Zaměstnanec podle e-mailu |
| `DELETE` | `/api/organizations/employees/{employeeId}` | Odebrání zaměstnance z organizace |

### Majetek, položky, návrhy, inventury

| Metoda | Cesta | Popis |
|---|---|---|
| `GET` | `/api/properties` | Aktuální majetek (podle role) |
| `POST` | `/api/properties` | Vytvoření majetku |
| `PUT` | `/api/properties` | Aktualizace majetku |
| `DELETE` | `/api/properties` | Smazání majetku |
| `GET` | `/api/properties/items` | Seznam majetkových položek (filtry dle role) |
| `GET` | `/api/properties/items/{itemId}` | Položka podle ID |
| `POST` | `/api/properties/items` | Vytvoření položek |
| `PUT` | `/api/properties/items` | Aktualizace položek |
| `DELETE` | `/api/properties/items` | Smazání vybraných položek nebo všech (`wipe=true`) |
| `POST` | `/api/properties/items/json-file` | Import položek z JSON |
| `POST` | `/api/properties/items/excel-file` | Import položek z Excelu (vyžaduje `columns` query) |
| `POST` | `/api/properties/items/generate-codes` | Vygeneruje ZIP s QR kódy (`size` query volitelné, default 150) |
| `PUT` | `/api/properties/items/{itemId}/scan` | Označení položky jako naskenované (`scannedWithCode` query) |
| `GET` | `/api/properties/items/export/excel` | Export položek do Excelu |
| `GET` | `/api/properties/items/export/json` | Export položek do JSON |
| `GET` | `/api/properties/suggestions` | Seznam návrhů změn (filtry dle role) |
| `POST` | `/api/properties/suggestions` | Vytvoření návrhu |
| `PUT` | `/api/properties/suggestions/{suggestionId}` | Aktualizace návrhu |
| `PUT` | `/api/properties/suggestions/{suggestionId}/accept` | Schválení návrhu |
| `PUT` | `/api/properties/suggestions/{suggestionId}/decline` | Zamítnutí návrhu |
| `DELETE` | `/api/properties/suggestions/{suggestionId}` | Smazání návrhu |
| `GET` | `/api/properties/scans` | Seznam inventur (admin) |
| `GET` | `/api/properties/scans/active` | Aktivní inventura (podle role) |
| `GET` | `/api/properties/scans/{scanId}/items` | Seznam položek v inventuře |
| `POST` | `/api/properties/scans` | Vytvoření inventury |
| `PUT` | `/api/properties/scans` | Aktualizace inventury |
| `PUT` | `/api/properties/scans/complete` | Dokončení aktivní inventury |

## Nastavení databáze a migrace

### Lokální Docker flow

- PostgreSQL běží jako služba `postgres` v `docker-compose.dev.yml`.
- API používá `ConnectionStrings__Connection` z env/config.

### EF Core migrace

Migrační projekt:

- `backend/src/InvenireServer.Infrastructure`

Pomocné Make targety:

```bash
make update_database
make add_migration name=YourMigrationName
make drop_database
```

### Chování při startu aplikace

- v **non-production** prostředí se pending migrace aplikují automaticky při startu
- v **production** je automatická migrace vypnutá

## Struktura složek

```text
backend/
├─ readme.md
├─ LICENSE
├─ docker-compose.dev.yml
├─ Dockerfile.dev
├─ Makefile
├─ assets/
│  ├─ fonts/
│  └─ templates/
├─ logs/
├─ src/
│  ├─ InvenireServer.Application/
│  ├─ InvenireServer.Domain/
│  ├─ InvenireServer.Infrastructure/
│  └─ InvenireServer.Presentation/
└─ tests/
   └─ InvenireServer.Tests/
```

### Shrnutí vrstev

| Složka | Účel |
|---|---|
| `src/InvenireServer.Presentation` | API host, controllery, middleware, startup wiring |
| `src/InvenireServer.Application` | use-casy, CQRS handlery, validátory, service kontrakty |
| `src/InvenireServer.Domain` | entity, doménové konstanty, core výjimky |
| `src/InvenireServer.Infrastructure` | persistence, autentizace, e-mail, QR utility |
| `tests/InvenireServer.Tests` | unit a integrační testy |

## Testování

Spuštění všech backend testů:

```bash
cd backend
make test
```

Nebo přímo:

```bash
dotnet test ./tests/InvenireServer.Tests/InvenireServer.Tests.csproj
```

## Licence

Tento backend je licencovaný pod MIT licencí. Viz `backend/LICENSE`.
