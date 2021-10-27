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

    public void OnGet()
    {
      todos = todo.GetToDos().GetAwaiter().GetResult();
    }

    public void OnPostInsert()
    {
      var todoTask = Request.Form["todotask"];

      todo.PostToDo(todoTask).GetAwaiter().GetResult();
      todos = todo.GetToDos().GetAwaiter().GetResult();
    }

    public void OnPostMarkDone(string id)
    {      
      todo.MarkDoneToDo(id).GetAwaiter().GetResult();
      todos = todo.GetToDos().GetAwaiter().GetResult();

      RedirectToPage("/ToDoList");
    }

    public void OnPostDeleteToDo(string deleteId)
    {
      todo.DeleteToDo(deleteId).GetAwaiter().GetResult();
      todos = todo.GetToDos().GetAwaiter().GetResult();

      RedirectToPage("/ToDoList");
    }
  }
}
