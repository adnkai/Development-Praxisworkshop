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

### From the /Fronted directory
``` powershell
 dotnet publish -c Release -o ./myapp
 zip -r deploy.zip ./myapp
 az webapp deploy --resource-group <group-name> --name <app-name> --src-path deploy.zip
```

### NuGet Package Manager Extension reparieren
``` powershell 
rm -r "C:\Users\<username>\AppData\Roaming\NuGet"
dotnet restore
```


### .Net 6 in der CloudShell installieren
``` powershell
 wget -q -O - "https://dot.net/v1/dotnet-install.sh" | bash -s -- --version 6.0.101
 export PATH="~/.dotnet:$PATH"
 echo "export PATH=~/.dotnet:\$PATH" >> ~/.bashrc
```
# Vorbereitung für lokales Arbeiten
Mit folgenden Tools werden wir im Workshop arbeiten:

- Dotnet 3.1/6.0
  - https://dotnet.microsoft.com/en-us/download/dotnet/3.1 (not necessarily)
  - https://dotnet.microsoft.com/en-us/download/dotnet/6.0
- Azure Function Core Tools (v4)
  - https://docs.microsoft.com/de-de/azure/azure-functions/functions-run-local?tabs=v4
- Azure CLI 
  - https://docs.microsoft.com/de-de/cli/azure/install-azure-cli
- Visual Studio Code
  - https://code.visualstudio.com/Download
- Git
  - https://git-scm.com/downloads

### Github Actions
# Flag file as executable
git update-index --chmod=+x script.sh