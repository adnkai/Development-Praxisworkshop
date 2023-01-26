# Development-Praxisworkshop
ADN Praxisworkshop zum Thema Azure Development

In diesem 2-Tages Workshop steigen wir in die Software-Entwicklung von Azure Cloud-Lösungen ein.
Wir erstellen eine WebApp mit Authentication, binden externe Ressourcen ein und nutzen noSQL Speicher in Azure.
Außerdem richten wir Monitoring ein, lagern Informationen in einen Cache aus und lassen am Ende alles durch eine CI/CD Pipeline bereitstellen.

# Inhalt:
- Azure App Service
- Storage Accounts 
- Azure Key Vault
- Azure Functions
- Managed Identity/App Registration
- Azure Cache for Redis
- Azure App Configuration
- Authentication with Azure AD
- Monitoring with Application Insights
- Azure DevOps Pipelines
- GitHub Actions

# Voraussetzungen
Erfahrungen im Programmieren und Git werden vorausgesetzt.
Die Kursinhalte werden in C# erarbeitet.

ADN Shop: [ADN Praxis Workshop - Azure Development](https://shop.adn.de/Hersteller/Microsoft-ADN/ADN-Praxis-Workshop-Azure-Development.html)

# Disclaimer
Der hier hinterlegte Code, inkl. aller Beispiele und Konfigurationsdateien dient ausschließlich schulischen Zwecken und sollte nicht ohne weitere Prüfung für produktive Szenarien verwendet werden.

# Troubleshooting
Azure Cloud Shell für zip deploy der Anwendung:

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

- Net 6
  - https://dotnet.microsoft.com/en-us/download/dotnet/3.1 (not necessary anymore)
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

# Release-Annotations.sh
Ein Script für Release Annotations in Application Insights
