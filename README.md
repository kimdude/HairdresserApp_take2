# DT191G - Webbutveckling med .NET
## Projektuppgift - Bokningssystem för frisörsalong

Repot innehåller källkod av ett bokningssystem för en frisörsalong. Systemet innehåller ett step-by-step bokningssystem med kalender över aktuell eller kommande vecka.
Vid bokning kan de frisörer väljas som erbjuder tjänsten. I kalendern visas då tider som vald frisör inte är uppbokad. Vid skapande, uppdatering eller radering av bokning skickas e-postbekräftelser ut till kunden.
Utöver hantering av bokningar kan även frisörer och tjänster hanteras.

Systemet är utvecklat som ASP.NET MVC-applikation. Den använder EF Core Identity för autentisering. För styling används Bootstrap.

### Databastabeller
__Hairdressers__
Operation  | Syfter                                                         | Autentisering  |
-----------|----------------------------------------------------------------|----------------|
Create     | Skapa ny frisör i samband med registrering av ny användare.    | Autentisering  |
Read       | Hämta samtliga frisörer.                                       | Autentisering  |
Update     | Uppdatera frisör (Ej användare).                               | Autentisering  |
Delete     | Radera frisör och kopplad användare.                           | Autentisering  |

__Services__
Operation  | Syfter                                                         | Autentisering  |
-----------|----------------------------------------------------------------|----------------|
Create     | Skapa ny tjänst.                                               | Autentisering  |
Read       | Hämta samtliga tjänster.                                       | Autentisering  |
Update     | Uppdatera tjänst.                                              | Autentisering  |
Delete     | Radera tjänst (soft delete).                                   | Autentisering  |

__OfferedServices__
Tabellen är en junction table från many-to-many relationen mellan *Services* och *Hairdressers*.
Operation  | Syfter                                                                 | Autentisering  |
-----------|------------------------------------------------------------------------|----------------|
Create     | Skapa ny tillgänglig tjänst (Lägg till frisör som erbjuder tjänsten).  | Autentisering  |
Read       | Hämta tillgängliga tjänster.                                           |                |
Delete     | Ta bort tillgänglig tjänst (D.v.s. frisör som erbjuder tjänsten).      | Autentisering  |

__Bookings__
Operation  | Syfter                                                                   | Autentisering  |
-----------|--------------------------------------------------------------------------|----------------|
Create     | Skapa ny bokning. Skickar e-postmeddelanden till kund.                   |                |
Read       | Hämta bokningar från dagens datum eller kommande vecka.                  | Autentisering  |
Update     | Uppdatera frisör och tid på bokning. Skickar e-postmeddelande till kund. | Autentisering  |
Delete     | Radera bokning. Skickar e-postmeddelande till kund.                      | Autentisering  |

__Customers__
Tabellen stödjer i nuläget inga operationer. Den finns just nu främst till för att underlätta bokning för återkommande kunder och utskick av mejl.
Den finns även till för att implementera funktionalitet för sökning av bokningar framöver.

__BookedServices__
Tabellen stödjer i nuläget inga operationer. Den är en junction table från many-to-many relationen mellan *Services* och *Bookings*. Detta är främst för att möjliggöra tillägg av 
tjänster i samma bokning vid vidareutveckling framöver.

## Uppsättning & körning av projektet
För att köra projektet, klona repot med git clone. Projektet bygger på version 10 av .NET. 

Skapa lokal SQL-databasen genom projektets migrationer:
*dotnet ef database update*

Bygg projektet:
*dotnet build*

Öppna i förhandsvisning:
*dotnet watch* eller *dotnet run*

### Beroenden & konfigurationer
För utskick av e-postbekräftelser krävs en SMTP-server. Anslutningsdetaljer för dessa behöver lagras som User Secrets.

Initiera User Secrets:
*dotnet user-secrets init*

Lägg till följande secrets:
*dotnet user-secrets set "Port" "xxx"*
*dotnet user-secrets set "Host" "xxx"*
*dotnet user-secrets set "Mail" "xxx"*
*dotnet user-secrets set "Password" "xxx"*

__Kim Dudenhöfer, 2026__
