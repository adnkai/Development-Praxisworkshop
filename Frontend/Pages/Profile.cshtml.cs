namespace Development_Praxisworkshop.Pages;

[Authorize]
public class ProfileModel : PageModel
{
    #region Private
        private readonly ILogger<ProfileModel> _logger;
        private readonly GraphServiceClient _graphServiceClient;
        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        private HttpContext _httpContext;
        private string[] _graphScopes;
    #endregion

    public ProfileModel(ILogger<ProfileModel> logger, 
                        IConfiguration configuration,
                        GraphServiceClient graphServiceClient,
                        MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
                        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
        _httpContext = httpContextAccessor.HttpContext;

        this._consentHandler = consentHandler;
        
        _graphScopes = configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ')!;
    }

    public void OnGet()
    {
        var token = _httpContext.GetTokenAsync("OpenIdConnect", "access_token").Result;
        User currentUser = _graphServiceClient.Me.Request().GetAsync().GetAwaiter().GetResult();

        try
        {
            currentUser = _graphServiceClient.Me.Request().GetAsync().GetAwaiter().GetResult();
        }
        // Catch CAE exception from Graph SDK
        catch (ServiceException svcex) when (svcex.Message.Contains("Continuous access evaluation resulted in claims challenge"))
        {
            try
            {
                Console.WriteLine($"{svcex}");
                string claimChallenge = WwwAuthenticateParameters.GetClaimChallengeFromResponseHeaders(svcex.ResponseHeaders);
                _consentHandler.ChallengeUser(_graphScopes, claimChallenge);
            }
            catch (Exception ex2)
            {
                _consentHandler.HandleException(ex2);
            }
        }

        try
        {
            // Get user photo
            using (var photoStream = _graphServiceClient.Me.Photo.Content.Request().GetAsync().Result)
            {
                byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                ViewData["Photo"] = Convert.ToBase64String(photoByte);
            }
        }
        catch (Exception pex)
        {
            Console.WriteLine($"{pex.Message}");
            ViewData["Photo"] = null;
        }

        ViewData["Me"] = currentUser;
    }
}