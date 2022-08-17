

namespace Development_Praxisworkshop.Pages;

[AllowAnonymous]
public class _LayoutModel : PageModel
{
    private readonly ILogger<_LayoutModel> _logger;
    private readonly TelemetryClient _telemetryClient;
    private readonly IFeatureManager _featureManager;
    
    public bool showOneDrive;

    public _LayoutModel(ILogger<_LayoutModel> logger, TelemetryClient telemetryClient, IFeatureManager featureManager)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
        _featureManager = featureManager;
        showOneDrive = _featureManager.IsEnabledAsync("ShowOneDrive").Result;
    }
    public void OnGet()
    {
        _telemetryClient.TrackPageView("Layout");
    }
}

