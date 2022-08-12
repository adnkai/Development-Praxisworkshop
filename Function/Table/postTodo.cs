namespace Project;
public static class postTodo
{
  [FunctionName("postElement")]
  public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "postElement/{tableName}")] HttpRequest req,
      ILogger log, string tableName)
  {   
    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    string _upn = req.Query["upn"];

    var data = JsonConvert.DeserializeObject<ListElementModel>(requestBody);
    data.PartitionKey = tableName;
    
    ListElementModel result;
    
    try
    {
      result = await (new TableSettings(_upn)).InsertItem(tableName, data);
    }
    catch (Exception e)
    {
      return new BadRequestObjectResult(e.Message);
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

    log.LogInformation($"UPN: {_upn}");
    log.LogInformation($"table: {tableName}");

    bool result;
    
    try
    {
      result = await (new TableSettings(_upn)).CreateTableForUser(tableName);
    }
    catch (Exception e)
    {
      return new BadRequestObjectResult(e.Message);
    }

    return new OkObjectResult(tableName);
  }
}