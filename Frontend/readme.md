# Frontend


Die Frontend-Anwendung zum Development Praxisworkshop.
Mit dieser Anwendung demonstrieren wir in kleineren Teilen die Integration unterschiedlicher Azure-Services in eine einfache Webanwendung.

---

**01 - App Service**

Wir erklären Azure Web Apps und erstellen eine simple Webanwendung. Anschließend zeigen wir unterschiedliche Möglichkeiten auf, diese in Azure bereitzustellen.

**02 - Storage Accounts**

Wir erzählen euch etwas zum sogenannten *Azure Speicherkonto* und integrieren persistenten Speicher in unsere Webanwendung in Form einer Bildergallerie und einer To-Do-Liste.

**03 - Azure Functions**

Wir zeigen und erklären euch *Azure Functions*. Im Workshop verwenden wir sie, um die Funktionalitäten der To-Do-Liste auszulagern und ein gewisses Maß an Basis-Security einzubauen.

**04 - Key Vault und Managed Identities**

Mit dem Azure Key Vault verwalten wir unsere Secrets und Passwörter. Mit Managed Identities implementieren wir einen Passwordlosen Zugriff über die Azure Function auf den Storage Account.

**05 - Authentication**

Wir implementieren Azure AD Authentication (oAuth) in unsere App und spielen ein wenig mit Berechtigungen.

**06 - Azure Monitoring**

Mit Azure Monitoring und *Application Insights* bilden wir das Thema Monitoring ab und schauen uns Statistiken über unsere WebApp an.

**07 - Azure App Configuration**

Mit Azure App Configuration lösen wir die statischen Konfigurationen aus den Services und schaffen eine zentrale Verwaltung.
Außerdem: Feature Flags!

**08 - Pipelines**

Der Workshop wird abgerundet, indem wir eine CI/CD Pipeline über GitHub Actions erstellen und unser App-/Function-Deployment über Push-Trigger automatisieren.
Außerdem zeigen wir den Weg auch über Azure DevOps Pipelines.
