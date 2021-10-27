using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Project
{
  public static class postTodo
  {
    [FunctionName("postTodo")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("Insert new Todo");
      
      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      var partitionKey = System.Environment.GetEnvironmentVariable("TABLE_PARTITION_KEY", EnvironmentVariableTarget.Process);
      var data = JsonConvert.DeserializeObject<Todo>(requestBody);
      Todo result;
      
      try
      {
        result = await (new TableSettings(partitionKey)).InsertItem(data);
      }
      catch (Exception)
      {
        return new BadRequestObjectResult(null);
      }

      return new OkObjectResult(result);
    }
  }
}
