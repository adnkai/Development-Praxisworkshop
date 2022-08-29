namespace Development_Praxisworkshop.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    #region Private
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;
        private readonly IConfigurationRefresher _refresher;
    #endregion

    public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IConfigurationRefresher refresher)
    {
        _config = config;
        _logger = logger;
        _refresher = refresher;
    }
    public void OnGet(int? id){
        
        // /index?id=1
        if (id != null) {
            _refresher.TryRefreshAsync();
        }
    }

}