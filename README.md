# PINrad

Boris Sabljić
3.9.2024.

PINrad aplikacija

Kratak opis aplikacije:

Aplikacija za upravljanje evidencijama odnosno vođenje evidencije imovine. 
Aplikacija podržava funkcionalnosti kao što su dodavanje, uređivanje, brisanje i pregled imovine, kao i zaduženja imovine. 
Moguće je pretraživati po imenu korisnika ili nazivu imovine. 
Aplikacija također omogućuje izvoz podataka u Excel i PDF formate te uvoz podataka iz Excel datoteka. 
Implementirana je i autentifikacija korisnika, a pristup određenim funkcionalnostima je ograničen na temelju korisničkih uloga (malo mi ovaj dio je problematičan ali radi).
Neke stvari kao promjena lozinke, moj profil i slično nije dodano. NO važno da funkcionira sve.
Korišteno za izradu:

Za potrebe izrade ove aplikacije upotrjebljeno je:
1. Microsoft Visual Studio Community 2022
1.1. Realizacija putem ASP.NET Core MVC
2. Microsoft SQL Server 2022
3. Microsoft SQL Server Management Studio 20.8
Korišteni NuGet paketi:

Ova aplikacija koristi ove NuGet pakete kako bi omogućila rad s bazama podataka, autentifikaciju korisnika, te rad s Excel i PDF datotekama:
1. Microsoft.EntityFrameworkCore (rad sa bazom)
2. Microsoft.EntityFrameworkCore.SqlServer (rad sa bazom)
3. Microsoft.EntityFrameworkCore.Tools (rad sa bazom)
4. Microsoft.AspNetCore.Identity.EntityFrameworkCore (rad sa prijavama)
5. Microsoft.AspNetCore.Identity.UI (rad sa prijavama)
6. Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation (razor prikazi)
7. EPPlus (Excell)
8. itext7 (PDF)
9. itext7.bouncy-castle-adapter
11. EPPlus.System.Drawing (Opcionalno, za rad s grafikom u Excelu)
12.itext7.pdfhtml
13.System.Linq
14. System.Linq.Queryable


Manje važni:
Microsoft.Extensions.DependencyInjection.Abstractions (uključeno rad sa ovisnostima)
Microsoft.AspNetCore.Authentication.Cookies (autoraizacija putem kolačića)
Microsoft.AspNetCore.Authorization (autoraizacija)
System.Linq.Dynamic.Core (pretraživanje)

Inicijalizacija i update/kreiranje baze
add-migration nesto
update-database
remove-migration

ZA manipuliranje SQL query
SELECT * FROM AspNetUsers WHERE Email = 'testuser@pinrad.com';
SELECT LockoutEnabled, LockoutEnd, AccessFailedCount FROM AspNetUsers WHERE Email = 'testuser@pinrad.com';
UPDATE AspNetUsers SET LockoutEnabled = 0, AccessFailedCount = 0, LockoutEnd = NULL WHERE Email = 'testuser@pinrad.com';

CMD
TREE /A /F 
Objašnjenje strukture

|   PINrad.sln                           # Ovo je solution datoteka koja grupira projekte u Visual Studio-u, i preko nje mogu otvoriti cijeli projekt.
|   
\---PINrad
    |   appsettings.Development.json      # Ova datoteka sadrži postavke za razvojno okruženje, uključujući parametre specifične za razvoj.
    |   appsettings.json                  # Glavna konfiguracijska datoteka aplikacije, gdje postavljam različite parametre poput veze s bazom podataka.
    |   PINrad.csproj                     # Ova datoteka definira projekt i njegove ovisnosti, uključujući NuGet pakete.
    |   PINrad.csproj.user                # Sadrži postavke specifične za mene kao korisnika, obično se ne dijeli s drugima.
    |   Program.cs                        # Glavna ulazna točka aplikacije, ovdje postavljam kako će se aplikacija pokrenuti.
    |   
    +---Areas
    |   \---Identity
    |       \---Pages
    |               _ViewStart.cshtml     # Ova datoteka postavlja osnovne postavke za prikaz stranica unutar identiteta (login, registracija itd.).
    |               
    +---Controllers
    |       AccountController.cs          # Kontroler koji upravlja logikom autentifikacije korisnika, poput prijave i odjave.
    |       AssetController.cs            # Ovdje sam postavio logiku za rad s imovinom (Asset), uključujući kreiranje, prikaz, izmjenu i brisanje.
    |       CustomUserController.cs       # Kontroler za rad s prilagođenim korisnicima u aplikaciji.
    |       HomeController.cs             # Glavni kontroler za osnovne stranice, poput početne stranice i stranica s informacijama.
    |       
    +---Data
    |   |   ApplicationDbContext.cs       # Ova klasa definira kontekst baze podataka, gdje upravljam entitetima i tablicama u bazi.
    |   |   DbInitializer.cs              # Datoteka koja inicijalizira bazu podataka, obično kreira početne podatke.
    |   |   
    |   \---Migrations
    |       ApplicationDbContextModelSnapshot.cs   # Ova datoteka prati trenutni model baze podataka i pomaže Entity Framework-u u praćenju promjena.
    +---Migrations
    |       ApplicationDbContextModelSnapshot.cs   # Koristi se za praćenje promjena u modelu baze podataka i olakšava migracije.
    |       
    +---Models
    |       Asset.cs                      # Definirao sam klasu koja predstavlja imovinu (Asset) u aplikaciji.
    |       Assignment.cs                 # Model koji predstavlja zadatke ili povezivanje korisnika s imovinom.
    |       CustomUser.cs                 # Ovaj model predstavlja prilagođene korisnike aplikacije, s posebnim poljima koja su mi potrebna.
    |       ErrorViewModel.cs             # Koristim ovaj model za prikazivanje informacija o greškama na stranici.
    |       HomeIndexViewModel.cs         # ViewModel za početnu stranicu, koji koristim za prikaz podataka na home stranici.
    |       LoginViewModel.cs             # ViewModel za prijavu, koji sadrži potrebne podatke za login formu.
    |       RegisterViewModel.cs          # ViewModel za registraciju novih korisnika, koji koristim na registracijskoj formi.
    |       RegLogUser.cs                 # Ovaj model spaja podatke o registraciji i prijavi korisnika.
    |       
    +---Views
    |   |   _ViewImports.cshtml           # Ova datoteka služi za uvoz zajedničkih dijelova koda na sve stranice (npr. helperi i tagovi).
    |   |   _ViewStart.cshtml             # Definira zajedničke postavke za sve prikaze, kao što je osnovni layout.
    |   |   
    |   +---Account
    |   |       AccessDenied.cshtml       # Stranica koja se prikazuje kada korisnik nema pristup određenom dijelu aplikacije.
    |   |       Login.cshtml              # Stranica za prijavu korisnika u sustav.
    |   |       Logout.cshtml             # Stranica koja se koristi za odjavu korisnika.
    |   |       Register.cshtml           # Stranica za registraciju novih korisnika.
    |   |       
    |   +---Asset
    |   |       Create.cshtml             # Stranica za kreiranje nove imovine.
    |   |       Delete.cshtml             # Stranica za brisanje imovine.
    |   |       Details.cshtml            # Prikaz detalja o odabranoj imovini.
    |   |       Edit.cshtml               # Stranica za izmjenu postojećih podataka o imovini.
    |   |       ImportExport.cshtml       # Stranica za uvoz i izvoz podataka o imovini.
    |   |       Index.cshtml              # Glavna stranica koja prikazuje popis svih unesenih imovina.
    |   |       
    |   +---CustomUser
    |   |       Create.cshtml             # Stranica za kreiranje novog prilagođenog korisnika.
    |   |       Delete.cshtml             # Stranica za brisanje korisnika.
    |   |       Details.cshtml            # Prikaz detalja o određenom korisniku.
    |   |       Edit.cshtml               # Stranica za izmjenu korisničkih podataka.
    |   |       Index.cshtml              # Prikaz popisa svih korisnika u aplikaciji.
    |   |       Report.cshtml             # Stranica za prikaz izvještaja o korisnicima.
    |   |       
    |   +---Home
    |   |       About.cshtml              # Stranica s informacijama o aplikaciji, obično "O nama".
    |   |       Error.cshtml              # Stranica koja se prikazuje kad dođe do greške u aplikaciji.
    |   |       Index.cshtml              # Početna stranica aplikacije.
    |   |       
    |   \---Shared
    |           _Layout.cshtml            # Glavna layout datoteka koja definira zajednički izgled svih stranica u aplikaciji.
    |           _Layout.cshtml.css        # CSS datoteka za stiliziranje glavnog layout-a.
    |           _LoginPartial.cshtml      # Djelomični prikaz za login formu koja se pojavljuje na raznim stranicama.
    |           _ValidationScriptsPartial.cshtml   # Učitava JavaScript validacijske skripte koje se koriste za validaciju unosa.
    |           
    \---wwwroot
        |   favicon.ico                   # Ikona koja se prikazuje na tabu preglednika.
        |   
        +---css
        |       site.css                  # Glavna CSS datoteka za stiliziranje stranica.
        |       
        +---js
        |       site.js                   # Glavna JavaScript datoteka za funkcionalnosti na  stranicama.

