namespace Project;

public static class deleteRoutes
{
  [FunctionName("deleteElement")]
  public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "deleteElement/{tableName}/{id}")] HttpRequest req,
      ILogger log, string tableName, string id)
  {
    log.LogInformation("Delete ListElementModel");

    string _upn = req.Query["upn"];

    try
    {
      await (new TableSettings(_upn)).DeleteItem(tableName, id);
    }
    catch (Exception e)
    {
      return new BadRequestObjectResult(e.Message); 
    }

    return new OkObjectResult(id);
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
    catch (Exception e)
    {
      return new BadRequestObjectResult(e.Message); 
    }

    return new OkObjectResult(tableName);
  }
}