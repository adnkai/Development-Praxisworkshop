namespace Development_Praxisworkshop.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private IUserDataSingleton _userdata;

    public IndexModel(ILogger<IndexModel> logger, IUserDataSingleton userDataSingleton)
    {
        _logger = logger;
        _userdata = userDataSingleton;
    }
    public void OnGet()
    {
        _userdata.upn = User.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
        Console.WriteLine(_userdata.upn);
    }
}