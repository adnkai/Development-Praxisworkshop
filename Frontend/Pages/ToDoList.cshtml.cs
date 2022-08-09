namespace Development_Praxisworkshop.Pages;

// [AllowAnonymous]
//[Authorize(Policy = "ClaimsTest")]
[Authorize]

public class ToDoListModel : PageModel
{
  private readonly ILogger<PrivacyModel> _logger;
  private readonly IConfiguration _config;
  public Dictionary<String, List<TodoModel>> todos;
  public List<TablesTableModel> todoLists;
  private static TableAccountHelper todo;
  private readonly TelemetryClient _telemetryClient;
  private IHttpContextAccessor _httpContextAccessor;
  private ClaimsPrincipal _user;

  public ToDoListModel(ILogger<PrivacyModel> logger, IConfiguration config, TelemetryClient telemetryClient, IHttpContextAccessor httpContextAccessor)
  {
    _telemetryClient = telemetryClient;
    _logger = logger;
    _config = config;
    todos = new Dictionary<String, List<TodoModel>>();
    _httpContextAccessor = httpContextAccessor;
    _user = _httpContextAccessor.HttpContext.User;
    todo = new TableAccountHelper(_config, _telemetryClient, _user);
  }

  public async Task<IActionResult> OnGetAsync()
  {
    await Task.Run(() => {
      todos = todo!.GetToDos().Result;
      todoLists = todo!.GetToDoLists();
    });
    return Page();
  }

  public async Task<IActionResult> OnPostInsertAsync(string todoTask, string listName)
  {
    if (String.IsNullOrEmpty(todoTask))
    {
      return RedirectToPage("/ToDoList");
    }

    var model = new TodoModel();
    model.PartitionKey = listName;
    model.TaskDescription = todoTask;

    await todo!.PostToDo(model, listName);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostMarkDoneAsync(string rowkey, string listName)
  {
    await todo!.MarkDoneToDo(rowkey, listName);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostDeleteToDoAsync(string rowKey, string listName)
  {
    await todo!.DeleteToDo(rowKey, listName);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostCreateToDoListAsync(string todolistname)
  {
    // string upn = User.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
    await todo!.PostCreateToDoList(todolistname);
    
    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostDeleteToDoListAsync(string todolistname)
  {
    // string upn = User.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
    await todo!.PostDeleteToDoList(todolistname);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostArchiveToDoListAsync(string listName)
  {
    await todo!.ArchiveList(listName);

    return RedirectToPage("/ToDoList");
  }
}
