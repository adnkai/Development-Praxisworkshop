namespace Development_Praxisworkshop.Helper;

public class TableAccountHelper
{
  private CloudTableClient _tableClient;
  private CloudTable _table;
  readonly TelemetryClient _telemetryClient;

  public TableAccountHelper(IConfiguration config, TelemetryClient telemetry)
  {
    _telemetryClient = telemetry;

    CloudStorageAccount _acc = CloudStorageAccount.Parse(config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"));
    _tableClient = _acc.CreateCloudTableClient();

    CloudTable table = _tableClient.GetTableReference(config.GetSection("StorageAccount").GetValue<string>("TablePartitionKey"));
    table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
    _table = table;
  }

  public List<TodoModel> GetToDos()
  {
    _telemetryClient.TrackEvent("ListTodo");
    _telemetryClient.TrackTrace("TraceMessage GetToDos");
    return EnumerateDocumentsAsync(_table);
  }

  public async Task<TodoModel> PostToDo(TodoModel _todo)
  {
    _telemetryClient.TrackEvent("CreateTodo");
    _telemetryClient.TrackMetric("CUSTOM_CreatedTodo_DescriptionLength", _todo.TaskDescription.Length);
    _telemetryClient.TrackTrace("TraceMessage CreateToDos");
    return await InsertItem(_todo);
  }

  public async Task<TodoModel> MarkDoneToDo(string _rowKey)
  {
    _telemetryClient.TrackEvent("MarkDoneTodo");
    return await UpdateToDo(_rowKey);
  }
  public async Task<TableResult> DeleteToDo(string _rowKey)
  {
    _telemetryClient.TrackEvent("DeleteTodo");
    _telemetryClient.TrackTrace("TraceMessage DeleteToDos");
    _telemetryClient.TrackException(new OverflowException());
    return await DelToDo(_rowKey);
  }

  private List<TodoModel> EnumerateDocumentsAsync(CloudTable _table)
  {
    List<TodoModel> tmpTodos = new List<TodoModel>();
    TableQuery<TodoModel> query = new TableQuery<TodoModel>();

    foreach (TodoModel todo in _table.ExecuteQuery(query))
    {
      tmpTodos.Add(todo);
    }
    tmpTodos.Sort((x, y) => x.TaskDescription.CompareTo(y.TaskDescription));
    _telemetryClient.TrackMetric("CUSTOM_ListTodo_Count", tmpTodos.Count);
    return tmpTodos;
  }

  private async Task<TodoModel> InsertItem(TodoModel _todoItem)
  {
    if (_todoItem == null)
    {
      throw new NullReferenceException();
    }

    TableResult result;
    TableOperation operation = TableOperation.InsertOrReplace(_todoItem);
    
    try
    {
      result = await _table.ExecuteAsync(operation);
      _telemetryClient.TrackEvent("InsertItem");
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }

    TodoModel newTodo = (TodoModel)result.Result;

    return newTodo;
  }

  private async Task<TodoModel> UpdateToDo(string _rowKey)
  {
    TableResult result;
    TableOperation operation = TableOperation.Retrieve<TodoModel>("TODO", _rowKey);
    TodoModel updatedToDo;

    try
    {
      result = await _table.ExecuteAsync(operation);

      if (result.Result == null)
      {
        throw new NullReferenceException();
      }

      TodoModel m = (TodoModel)result.Result;
      m.IsCompleted = m.IsCompleted ? false : true;

      updatedToDo = await InsertItem(m);
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
    _telemetryClient.TrackMetric("CUSTOM_MarkedTodo_Value", updatedToDo.IsCompleted == false ? 0 : 1);
    return updatedToDo;
  }

  private async Task<TableResult> DelToDo(string _rowKey)
  {
    try
    {
      TableOperation operation = TableOperation.Delete(new TodoModel(_rowKey, "TODO"));
      return await _table.ExecuteAsync(operation);
      
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
    }
    return null;
  }
}
