namespace Project;
public static class postTodo
{
  [FunctionName("postElement")]
  public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "postElement/{tableName}")] HttpRequest req,
      ILogger log, string tableName)
  {
    log.LogInformation("Insert new ListElementModel");
    
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

    var data = JsonConvert.DeserializeObject<ListElementModel>(requestBody);
    string _upn = req.Query["upn"];

    ListElementModel result;
    
    try
    {
      result = await (new TableSettings(_upn)).InsertItem(tableName, data);
    }
    catch (Exception)
    {
      return new BadRequestObjectResult(null);
    }

    return new OkObjectResult(result);
  }

  [FunctionName("postTable")]
  public static async Task<IActionResult> Run2(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "postTable/{tableName}")] HttpRequest req,
      ILogger log, string tableName)
  {
    log.LogInformation("Insert new Table");
    
    string _upn = req.Query["upn"];

    bool result;
    
    try
    {
      result = await (new TableSettings(_upn)).CreateTableForUser(tableName);
    }
    catch (Exception)
    {
      return new BadRequestObjectResult(null);
    }

    return new OkObjectResult(tableName);
  }
}