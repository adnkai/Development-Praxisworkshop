namespace Development_Praxisworkshop.Pages;

// [AllowAnonymous]
//[Authorize(Policy = "ClaimsTest")]
[Authorize]

public class ToDoListModel : PageModel
{
  private readonly ILogger<PrivacyModel> _logger;
  private readonly IConfiguration _config;
  public List<TodoModel> todos;
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
    todos = new List<TodoModel>();
    _httpContextAccessor = httpContextAccessor;
    _user = _httpContextAccessor.HttpContext.User;
    todo = new TableAccountHelper(_config, _telemetryClient, _user);
  }

  public async Task<IActionResult> OnGetAsync()
  {
    await Task.Run(() => {
      todos = todo!.GetToDos();
      todoLists = todo!.GetToDoLists();
    });
    return Page();
  }

  public async Task<IActionResult> OnPostInsertAsync(string todoTask)
  {
    if (String.IsNullOrEmpty(todoTask))
    {
      return RedirectToPage("/ToDoList");
    }

    var model = new TodoModel();
    model.TaskDescription = todoTask;

    await todo!.PostToDo(model);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostMarkDoneAsync(string id)
  {
    await todo!.MarkDoneToDo(id);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostDeleteToDoAsync(string deleteId)
  {
    await todo!.DeleteToDo(deleteId);

    return RedirectToPage("/ToDoList");
  }

  public async Task<IActionResult> OnPostCreateToDoListAsync(string todolistname)
  {
    // string upn = User.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
    await todo!.PostCreateToDoList(todolistname);

    return RedirectToPage("/ToDoList");
  }
}
