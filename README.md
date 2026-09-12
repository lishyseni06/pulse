# PULSE — Portal lajmesh

Aplikacion web **ASP.NET Core 8 MVC** (C#, .NET 8) me **Entity Framework Core 8**, **SQL Server**, **Bootstrap 5** dhe autentikim me **cookie** për panelin e adminit.

Slogan: **Lajme • Analiza • Informacione**

## Si të startosh

1. Sigurohu që SQL Server (instanca `MSSQLSERVER` në `localhost`) është aktive.
2. Nga dosja e projektit:

```bash
dotnet run
```

3. Hape shfletuesin te `http://localhost:5199` (ose porti që shfaqet në konsolë).

Në startim, aplikacioni **krijon vetë bazën `PulseDb`** dhe e mbush me kategori, adminin fillestar dhe lajme shembull (`Data/DbSeeder.cs`).

### Alternativë: skripti SQL
Për ta krijuar bazën manualisht (drop & create, idempotent):

```bash
sqlcmd -S localhost -E -i Database\Pulse.sql -f 65001
```

> `-f 65001` (UTF-8) është i domosdoshëm që shkronjat shqipe të ruhen saktë.

## Kyçja në panel
- URL: `/Admin/Login`
- Përdoruesi: **admin**
- Fjalëkalimi: **admin123**

## Struktura
```
/Controllers   → HomeController, AdminController
/Models        → News, Category, Admin, LoginViewModel
/Data          → AppDbContext, DbSeeder
/Services      → PasswordHasher (SHA-256), CategoryBadge
/Views         → Home (Index, List, Details), Admin (Login, Dashboard, Edit, Delete), Shared (_Layout)
/Database      → Pulse.sql
/wwwroot/css   → site.css
/wwwroot/images→ logo.jpeg
```

## Konfigurimi i lidhjes
Ndrysho `ConnectionStrings:DefaultConnection` në `appsettings.json` nëse përdor instancë tjetër SQL (p.sh. `.\SQLEXPRESS`).
