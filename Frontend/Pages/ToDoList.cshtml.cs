using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;

using Development_Praxisworkshop.Helper;

namespace Development_Praxisworkshop.Pages
{
  //[AllowAnonymous]
  [Authorize]
  public class ToDoListModel : PageModel
  {
    private readonly ILogger<PrivacyModel> _logger;
    private readonly IConfiguration _config;
    public List<TodoModel> todos;
    private static TableAccountHelper todo;
    private readonly TelemetryClient _telemetryClient;

    public ToDoListModel(ILogger<PrivacyModel> logger, IConfiguration config, TelemetryClient telemetryClient)
    {
      _telemetryClient = telemetryClient;
      _logger = logger;
      _config = config;
      todo = new TableAccountHelper(_config, _telemetryClient);
    }
    public async Task<IActionResult> OnGetAsync()
    {
      todos = todo.GetToDos();

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

      await todo.PostToDo(model);

      return RedirectToPage("/ToDoList");
    }

    public async Task<IActionResult> OnPostMarkDoneAsync(string id)
    {
      await todo.MarkDoneToDo(id);

      return RedirectToPage("/ToDoList");
    }

    public async Task<IActionResult> OnPostDeleteToDoAsync(string deleteId)
    {
      await todo.DeleteToDo(deleteId);

      return RedirectToPage("/ToDoList");
    }
  }
}
