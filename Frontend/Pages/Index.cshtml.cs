namespace Development_Praxisworkshop.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _config;
    private readonly IConfigurationRefresher _refresher;
    public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IConfigurationRefresher refresher)
    {
        _config = config;
        _logger = logger;
        _refresher = refresher;
    }
    public void OnGet(){
        _refresher.TryRefreshAsync();
    }
}