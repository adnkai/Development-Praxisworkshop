namespace Development_Praxisworkshop.Pages;

//[AllowAnonymous]
[Authorize]
public class ToDoListFunctionModel : PageModel
{
  #region Public
    public String listName;
    public Dictionary<String, List<ListElementModel>> todos;
    public List<CoreTableModel> todoLists;
  #endregion

  #region Private
    private readonly ILogger<PrivacyModel> _logger;
    private readonly IConfiguration _config;
    private static FunctionHelper _functionHelper;
    private readonly TelemetryClient _telemetryClient;
    private IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal _user;
    private IDistributedCache _distributetCache;
    private String _upn;
  #endregion

  public ToDoListFunctionModel(
    ILogger<PrivacyModel> logger, 
    IConfiguration config, 
    TelemetryClient telemetryClient, 
    IHttpContextAccessor httpContextAccessor,
    IDistributedCache distributedCache)
  {
    _telemetryClient = telemetryClient;
    _distributetCache = distributedCache;
    _logger = logger;
    _config = config;
    todos = new Dictionary<String, List<ListElementModel>>();
    _httpContextAccessor = httpContextAccessor;
    _user = _httpContextAccessor.HttpContext.User;
    _functionHelper = new FunctionHelper(_config, _telemetryClient, _user);
    _upn = _user.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
  }

  public async Task<IActionResult> OnGetAsync()
  {
    todos = await _functionHelper!.GetToDos();

    return Page();
  }

  public async Task<IActionResult> OnPostInsertAsync(string todoTask, string listName)
  {
    if (String.IsNullOrEmpty(todoTask))
    {
      return RedirectToPage("/ToDoListFunction");
    }

    await _functionHelper!.PostToDo(todoTask, listName);

    return RedirectToPage("/ToDoListFunction");
  }

  public async Task<IActionResult> OnPostMarkDoneAsync(string rowkey, string listName)
  {

    await _functionHelper!.Update(rowkey, listName);

    return RedirectToPage("/ToDoListFunction");
  }

  public async Task<IActionResult> OnPostDeleteToDoAsync(string rowkey, string listName)
  {
    await _functionHelper!.DeleteToDo(rowkey, listName);

    return RedirectToPage("/ToDoListFunction");
  }

  public async Task<IActionResult> OnPostCreateToDoListAsync(string listName)
  {
    await _functionHelper!.CreateToDoList(listName);

    return RedirectToPage("/ToDoListFunction");
  }

  public async Task<IActionResult> OnPostDeleteToDoListAsync(string listName)
  {
    await _functionHelper!.DeleteToDoList(listName);

    return RedirectToPage("/ToDoListFunction");
  }

}