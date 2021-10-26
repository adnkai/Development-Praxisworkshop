using System;
using System.Configuration;
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
  public static class deleteTodo
  {
    [FunctionName("deleteTodo")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("Delete Todo");

      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      dynamic data = JsonConvert.DeserializeObject(requestBody);

      var partitionKey = System.Environment.GetEnvironmentVariable("TABLE_PARTITION_KEY", EnvironmentVariableTarget.Process);
      Todo toDelete = new Todo((string)data?.RowKey, partitionKey);

      try
      {
        await (new TableSettings(partitionKey)).DeleteItem(toDelete);
      }
      catch (Exception)
      {
        return new BadRequestObjectResult(null);
      }

      return new OkObjectResult((string)data?.RowKey);
    }
  }
}
