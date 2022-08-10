namespace Project;

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