namespace Development_Praxisworkshop.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _config;
    private readonly IFeatureManager _featureManager;
    
    public bool showOneDrive;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IFeatureManager featureManager)
    {
        _config = config;
        _logger = logger;
        
        _featureManager = featureManager;
        showOneDrive = _featureManager.IsEnabledAsync("ShowOneDrive").Result;
    }
    public void OnGet(){
        Console.WriteLine(_config["TestApp:Settings:Message"] ?? "Hello world!");
    }
}