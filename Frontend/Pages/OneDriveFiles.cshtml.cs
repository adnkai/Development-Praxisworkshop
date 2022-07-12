namespace Development_Praxisworkshop.Pages;

//[Authorize(Policy="MustHaveOneDrive")] // need onedrive permissions
[AuthorizeForScopes(Scopes = new[]{"files.readwrite", "Sites.Read.All"})] // Welche scopes fordern wir auf dieser seite (ggf.) an?
public class OneDriveFilesModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;
    private readonly IConfiguration _config;
    public IDriveItemsCollectionPage files;
    readonly ITokenAcquisition _tokenAcquisition;
    private string _accessToken;
    private readonly GraphServiceClient _graphServiceClient;
    public IDriveItemChildrenCollectionPage _files;
    private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;

    private string[] _graphScopes;
    public OneDriveFilesModel(ILogger<PrivacyModel> logger, 
                                    IConfiguration config, 
                                    ITokenAcquisition tokenAcquisition,
                                    GraphServiceClient graphServiceClient,
                                    MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler)
    {
        _logger = logger;
        _config = config;
        _tokenAcquisition = tokenAcquisition;
        _graphServiceClient = graphServiceClient;
        this._consentHandler = consentHandler;
        _graphScopes = new[] {"files.readwrite", "Sites.Read.All"}; // required for Onedrive Items/Folders
    }

    public void OnGet()
    {
        // Scopes, welche JETZT benötigt werden.
        //string[] scopes = new string[]{"Contacts.Read","Family.Read"}; // Inkrementelle Anforderung automatisch durch bekanntgabe gaaaanz oben
        string[] scopes = new string[]{"files.readwrite", "Sites.Read.All"}; // Inkrementelle Anforderung automatisch durch bekanntgabe gaaaanz oben
        _accessToken = _tokenAcquisition.GetAccessTokenForUserAsync(scopes).Result;
        Console.WriteLine(_accessToken);
        _files = _graphServiceClient.Me.Drive.Root.Children.Request().GetAsync().Result;

    }

    
}