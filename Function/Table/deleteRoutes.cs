namespace Project;

public static class deleteRoutes
{
  [FunctionName("deleteElement")]
  public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deleteElement/{tableName}")] HttpRequest req,
      ILogger log, string tableName)
  {
    log.LogInformation("Delete ListElementModel");

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    dynamic data = JsonConvert.DeserializeObject<ListElementModel>(requestBody);
    string _upn = req.Query["upn"];

    try
    {
      await (new TableSettings(_upn)).DeleteItem(tableName, data);
    }
    catch (Exception)
    {
      return new BadRequestObjectResult(null); 
    }

    return new OkObjectResult((string)data?.RowKey);
  }

  [FunctionName("deleteTable")]
  public static async Task<IActionResult> Run2(
      [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deleteTable/{tableName}")] HttpRequest req,
      ILogger log, string tableName)
  {
    log.LogInformation("Delete Table");

    string _upn = req.Query["upn"];

    try
    {
      await (new TableSettings(_upn)).DeleteTableForUser(tableName);
    }
    catch (Exception)
    {
      return new BadRequestObjectResult(null); 
    }

    return new OkObjectResult(tableName);
  }
}