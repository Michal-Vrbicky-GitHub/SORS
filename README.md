# System Of River Stations
## Příkazy
#### vytvoření databáze
- Add-Migration SORS_DB -Context ApplicationDbContext -OutputDir Data/Migrations
- Update-Database -Verbose -Context ApplicationDbContext
#### použité NuGet
+ Install-Package Microsoft.EntityFrameworkCore.SqlServer
+ Install-Package Microsoft.EntityFrameworkCore.Tools
+ Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
+ Install-Package Swashbuckle.AspNetCore
+ Install-Package Microsoft.EntityFrameworkCore.Design -Version 7.0.4
+ Install-Package Hangfire -Version 1.7.31
+ Install-Package Hangfire.AspNetCore -Version 1.7.31
+ Install-Package Hangfire.SqlServer -Version 1.7.31

## Funkčnost
Záznamy ze stanic jsou řazeny takto, nahoře hodnoty přesahující LVLmax, řazeny od nejvyšších po nejnižší, po nich hodnoty nižší LVLmin, řazeny vzestupně (od největšího sucha dál), poté ostatní řazeny dle stáří, od nejmladších.

maximální stáří uchovaného reportu a periodu kontroly lze měnit v Services/ReportCleanupService

perioda porovnání dat reportů, za účelem odeslání upozornění mailem lze měnit v Services/ReportsCheck

chyba při odesílání mailu a neplatné reporty se zapisují do log.txt

Maily jsou za účelem testování zasílány na localhost:1025, lze změnit v appsettings.json, za pomoci programu mailhog je lze zobrazit ve webovém rozhraní na localhostu, port 8025 – http://127.0.0.1:8025, <br>
[mailhog stáhnout zde](https://github.com/mailhog/MailHog/releases/download/v1.0.0/MailHog_windows_amd64.exe), stačí jej jen spustit, nevyžaduje instalaci.

v grafu RiverComparison jsou data stanic na stejné řece – RiverName seskupena do jediného sloupce

#
Před spuštěním je nutno vytvořit databázi příkazy pro vytvoření databáze.


