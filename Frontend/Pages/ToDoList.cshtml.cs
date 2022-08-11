namespace Development_Praxisworkshop.Pages;

[AllowAnonymous]
[IgnoreAntiforgeryToken(Order = 1001)]

//[Authorize(Policy = "ClaimsTest")]
// [Authorize]

public class ToDoListModel : PageModel
{
  private readonly ILogger<PrivacyModel> _logger;
  private readonly IConfiguration _config;
  public Dictionary<String, List<ListElementModel>> todos;
  public List<CoreTableModel> todoLists;
  private static TableAccountHelper _tableAccountHelper;
  private readonly TelemetryClient _telemetryClient;
  private IHttpContextAccessor _httpContextAccessor;
  private ClaimsPrincipal _user;
  private IDistributedCache _distributetCache;
  private String _upn;
  public String listName;

  public ToDoListModel(
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
    _tableAccountHelper = new TableAccountHelper(_config, _telemetryClient, _user);
    _upn = _user.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
  }

  public async Task<IActionResult> OnGetAsync()
  {
    await Task.Run(() => {
      todos = _tableAccountHelper!.GetToDos().Result;
      todoLists = _tableAccountHelper!.GetToDoLists();
      listName = _distributetCache.GetStringAsync($"{_upn}:listName").Result;
    });
    return Page();
  }

  // Todos API Test
  public async Task<IActionResult> OnPost()
  {
      using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
      {
        
          var jsonContent = await reader.ReadToEndAsync();
          Console.WriteLine(jsonContent);
          
          return await PostedSomething();
      }
  }

  private async Task<IActionResult> PostedSomething()
    {
        return new EmptyResult();
    }

  public async Task<IActionResult> OnPostInsertAsync(string todoTask, string listName)
  {
    if (String.IsNullOrEmpty(todoTask))
    {
      return RedirectToPage("/ToDoList");
    }

    var model = new ListElementModel();
    model.PartitionKey = listName;
    model.TaskDescription = todoTask;
    
    await _distributetCache.RemoveAsync($"{_upn}:listName");
    await _distributetCache.SetStringAsync($"{_upn}:listName", listName);

    await _tableAccountHelper!.PostToDo(model, listName);
    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostMarkDoneAsync(string rowkey, string listName)
  {
    await _tableAccountHelper!.MarkDoneToDo(rowkey, listName);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostDeleteToDoAsync(string rowKey, string listName)
  {
    await _tableAccountHelper!.DeleteToDo(rowKey, listName);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostCreateToDoListAsync(string todolistname)
  {
    // string upn = User.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
    await _tableAccountHelper!.PostCreateToDoList(todolistname);
    
    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostDeleteToDoListAsync(string todolistname)
  {
    // string upn = User.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
    await _tableAccountHelper!.PostDeleteToDoList(todolistname);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostArchiveToDoListAsync(string listName)
  {
    await _tableAccountHelper!.ArchiveList(listName);

    return RedirectToPage("/ToDoList");
  }
}
