using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Development_Praxisworkshop.Helper;

namespace Development_Praxisworkshop.Pages
{
  public class ToDoListModel : PageModel
  {
    private readonly ILogger<PrivacyModel> _logger;
    private readonly IConfiguration _config;
    public List<TodoModel> todos;
    private static TableAccountHelper todo;

    public ToDoListModel(ILogger<PrivacyModel> logger, IConfiguration config)
    {
      _logger = logger;
      _config = config;
      todo = new TableAccountHelper(_config);
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
