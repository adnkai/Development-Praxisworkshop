namespace Development_Praxisworkshop.Helper;

public class TableAccountHelper
{
  private TableServiceClient _tableServiceClient;
  private TableClient _tableClient;
  readonly TelemetryClient _telemetryClient;

  private string _tableName = "TODO";

  public TableAccountHelper(IConfiguration config, TelemetryClient telemetry)
  {
    _telemetryClient = telemetry;

    // CloudStorageAccount _acc = CloudStorageAccount.Parse(config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"));
    // _tableClient = _acc.CreateCloudTableClient();

    _tableServiceClient = new TableServiceClient(config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"));
    //GetTableReference(config.GetSection("StorageAccount").GetValue<string>("TablePartitionKey"));
    _tableServiceClient.CreateTableIfNotExistsAsync(_tableName).GetAwaiter().GetResult();
    
    _tableClient = _tableServiceClient.GetTableClient(_tableName);

  }

  public List<TodoModel> GetToDos()
  {
    _telemetryClient.TrackEvent("ListTodo");
    _telemetryClient.TrackTrace("TraceMessage GetToDos");
    return EnumerateDocumentsAsync(_tableClient);
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

  public async Task<Azure.Response> DeleteToDo(string _rowKey)
  {
    _telemetryClient.TrackEvent("DeleteTodo");
    _telemetryClient.TrackTrace("TraceMessage DeleteToDos");
    _telemetryClient.TrackException(new OverflowException());
    return await DelToDo(_rowKey);
  }

  private List<TodoModel> EnumerateDocumentsAsync(TableClient _tableClient)
  {
    List<TodoModel> tmpTodos = new List<TodoModel>();

    // _tableClient.Query<TodoModel>();

    //TableQuery<TodoModel> query = new TableQuery<TodoModel>();

    foreach (TodoModel todo in _tableClient.Query<TodoModel>())
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

    try
    {
      Azure.Response result;
      result = await _tableClient.AddEntityAsync<TodoModel>(_todoItem);
      // result = await _table.ExecuteAsync(operation);
      _telemetryClient.TrackEvent($"InsertItem - {result}");
      // GET THIS ITEM FRESH
      return _todoItem;
      
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
    
  }

  private async Task<TodoModel> UpdateToDo(string _rowKey)
  {
    
    Pageable<TodoModel> queryResultsFilter = _tableClient.Query<TodoModel>(filter: $"RowKey eq '{_rowKey}'");

    Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");

    // TodoModel updatedToDo;
    Azure.Response response;
    TodoModel m;
    try
    {
      TodoModel item = queryResultsFilter.First();

      if (item == null)
      {
        throw new NullReferenceException();
      }

      m = item;
      m.IsCompleted = m.IsCompleted ? false : true;
      response = await _tableClient.UpdateEntityAsync<TodoModel>(m, ETag.All, TableUpdateMode.Merge);
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
    _telemetryClient.TrackMetric("CUSTOM_MarkedTodo_Value", m.IsCompleted == false ? 0 : 1);
    return m;
  }

  private async Task<Azure.Response> DelToDo(string _rowKey)
  {
    Azure.Response result;
    try
    {
      result = await _tableClient.DeleteEntityAsync("TODO", _rowKey);
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
    return result;
  }

  private async Task<TableClient> CreateToDoList(string _listName)
  {
    TableClient result;
    try
    {
      await _tableServiceClient.CreateTableIfNotExistsAsync(_tableName);
      result = _tableServiceClient.GetTableClient(_tableName);
      
      return result;
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
  }
}
