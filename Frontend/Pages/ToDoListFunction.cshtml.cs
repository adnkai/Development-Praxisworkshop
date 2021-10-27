using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using Development_Praxisworkshop.Helper;

namespace Development_Praxisworkshop.Pages
{
  [AllowAnonymous]
  //[Authorize]
  public class ToDoListFunctionModel : PageModel
  {
    private readonly ILogger<PrivacyModel> _logger;
    private readonly IConfiguration _config;
    public List<TodoModel> todos;
    private static FunctionHelper todo;

    public ToDoListFunctionModel(ILogger<PrivacyModel> logger, IConfiguration config)
    {
      _logger = logger;
      _config = config;
      todo = new FunctionHelper(_config);
    }

    public async Task<IActionResult> OnGetAsync()
    {
      todos = await todo.GetToDos();

      return Page();
    }

    public async Task<IActionResult> OnPostInsertAsync()
    {
      var todoTask = Request.Form["todotask"];

      await todo.PostToDo(todoTask);

      return RedirectToPage("/ToDoListFunction");
    }

    public async Task<IActionResult> OnPostMarkDoneAsync(string id)
    {      
      await todo.MarkDoneToDo(id);

      return RedirectToPage("/ToDoListFunction");
    }

    public async Task<IActionResult> OnPostDeleteToDoAsync(string deleteId)
    {
      await todo.DeleteToDo(deleteId);

      return RedirectToPage("/ToDoListFunction");
    }
  }
}
