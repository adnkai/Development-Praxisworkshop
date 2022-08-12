
namespace Project;
public static class getTodos
{
  [FunctionName("getAllElementsForUser")]
  public static async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getElements")] HttpRequest req,
      ILogger log)
  {
    log.LogInformation("Get All Elements For User");
    
    string _upn = req.Query["upn"];
    
    Dictionary<String, List<ListElementModel>> result;
    
    try
    {
      result = await (new TableSettings(_upn)).GetAllItemsForUser();
    }
    catch (Exception e)
    {
      return new BadRequestObjectResult(e.Message);
    }

    return new OkObjectResult(result);
  }

  [FunctionName("getAllElementInListForUser")]
  public static async Task<IActionResult> Run3(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getElements/{tableName}")] HttpRequest req,
      ILogger log, string tableName)
  {
    log.LogInformation(tableName);
    
    string _upn = req.Query["upn"];
    
    List<ListElementModel> result;
    
    try
    {
      result = await (new TableSettings(_upn)).GetItemsForTable(tableName);
    }
    catch (Exception e)
    {
      return new BadRequestObjectResult(e.Message);
    }

    return new OkObjectResult(result);
  }

   [FunctionName("getSingleElementInListForUser")]
  public static async Task<IActionResult> Run2(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "getElements/{tableName}/{id}")] HttpRequest req,
      ILogger log, string tableName, string id)
  {   
    string _upn = req.Query["upn"];
    
    ListElementModel result;
    
    try
    {
      result = await (new TableSettings(_upn)).GetItem(tableName, id);
    }
    catch (Exception e)
    {
      return new BadRequestObjectResult(e.Message);
    }

    return new OkObjectResult(result);
  }
}