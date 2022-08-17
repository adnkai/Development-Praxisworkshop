namespace Development_Praxisworkshop.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _config;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration config)
    {
        _config = config;
        _logger = logger;
    }
    public void OnGet(){
        Console.WriteLine(_config["TestApp:Settings:Message"] ?? "Hello world!");
    }
}