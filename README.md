# Development-Praxisworkshop
ADN Praxisworkshop zum Thema Azure Development

In diesem 2-Tages Workshop steigen wir in die Software-Entwicklung mithilfe von Cloud-Lösungen ein.
Wir erstellen eine WebApp mit Authentication, binden externe Ressourcen ein und nutzen noSQL Speicher in Azure.
Außerdem richten wir ein Basis-Monitoring ein, um zu zeigen, was in dieser Richtung alles möglich ist.

# Inhalt:
- Azure App Service
  - Was ist das?
  - App Service allgemein
  - Deployment

- Storage Account 
  - Was ist das?
  - App Service Integration
    - Bildergallerie
    - Todo-List

- Key Vault
  - Was ist das?
  - Sicherer Zugriff auf Geheimnisse
  - AppSettings

- Functions
  - Was ist das?
  - Sicherer Zugriff auf KeyVault-Geheimnisse
  - Auslagern von WebApp Funktionalitäten

- Managed Identity/App Registration
  - Was ist das?
  - Sicherer, passwordloser Zugriff auf Ressourcen

- Authentication
  - AzureAD OAuth implementierung

- Monitoring
  - Application Insights

- Pipelines
  - Pipeline-Integration unserer Anwendung
  - Automatisches Deployment

# Voraussetzungen
Erfahrungen im Programmieren und Git werden vorausgesetzt.
Die Kursinhalte werden in C# erarbeitet.

ADN Shop: [ADN Praxis Workshop - Azure Development](https://shop.adn.de/Hersteller/Microsoft-ADN/ADN-Praxis-Workshop-Azure-Development.html)

# Disclaimer
Der hier hinterlegte Code, inkl. aller Beispiele und Konfigurationsdateien dient ausschließlich schulischen Zwecken und sollte nicht ohne weitere Prüfung für produktive Szenarien verwendet werden.


# Troubleshooting
Using Azure Cloud Shell to zip deploy the application:

- From the /Fronted directory
  - dotnet publish -c Release -o ../myapp
  - zip -r deploy.zip ../myapp
  - az webapp deploy --resource-group <group-name> --name <app-name> --src-path deploy.zip

- nuget package manager extension
  - DELETE C:\Users\<username>\AppData\Roaming\NuGet directory, and then restore it using dotnet restore
