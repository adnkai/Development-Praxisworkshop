
namespace Project;
public static class getTodos
{
  [FunctionName("getTodos")]
  public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
      ILogger log)
  {
    log.LogInformation("Get Todos");
    
    var partitionKey = System.Environment.GetEnvironmentVariable("TABLE_PARTITION_KEY", EnvironmentVariableTarget.Process);
    string name = req.Query["name"];
    
    List<Todo> result;
    
    try
    {
      result = await (new TableSettings(partitionKey)).GetItems();
    }
    catch (Exception)
    {
      return new BadRequestObjectResult(null);
    }

    return new OkObjectResult(result);
  }

  [FunctionName("getTodo")]
  public static async Task<IActionResult> Run2(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getTodo/{id}")] HttpRequest req,
      ILogger log, string id)
  {
    log.LogInformation(id);
    
    var partitionKey = System.Environment.GetEnvironmentVariable("TABLE_PARTITION_KEY", EnvironmentVariableTarget.Process);
    string name = req.Query["name"];
    
    Todo result;
    
    try
    {
      result = await (new TableSettings(partitionKey)).GetItem(partitionKey, id);
    }
    catch (Exception)
    {
      return new BadRequestObjectResult(null);
    }

    return new OkObjectResult(result);
  }
}