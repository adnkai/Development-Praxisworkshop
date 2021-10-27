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
    //[AllowAnonymous]
    [Authorize]
    public class ToDoListModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IConfiguration _config;
        public List<TodoModel> todos;

        public ToDoListModel(ILogger<PrivacyModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public void OnGet()
        {
            TableAccountHelper todo = new TableAccountHelper(_config);
            todos = todo.GetToDos();
        }

        public void OnPostInsert()
        {
            var todoTask = Request.Form["todotask"];

            TableAccountHelper todo = new TableAccountHelper(_config);
            
            var model = new TodoModel();
            model.TaskDescription = todoTask;
            
            todo.PostToDo(model).GetAwaiter().GetResult();
            todos = todo.GetToDos();
        }

        public void OnPostMarkDone(string id)
        {           
           TableAccountHelper todo = new TableAccountHelper(_config);
                        
            todo.MarkDoneToDo(id).GetAwaiter().GetResult();
            todos = todo.GetToDos();

            RedirectToPage("/ToDoList");
        }

        public void OnPostDeleteToDo(string deleteId)
        {           
           TableAccountHelper todo = new TableAccountHelper(_config);
                        
            todo.DeleteToDo(deleteId).GetAwaiter().GetResult();
            todos = todo.GetToDos();

            RedirectToPage("/ToDoList");
        }
    }
}
