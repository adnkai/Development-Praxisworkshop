namespace Development_Praxisworkshop.Helper;

public class TableAccountHelper : PageModel
{
  private TableServiceClient _tableServiceClient;
  private TableClient _coreTableClient;
  private TableClient _tableClient;
  readonly TelemetryClient _telemetryClient;
  private ClaimsPrincipal _user;
  private String _upn;

  private string _coreTableName = "TablesTable";

  public TableAccountHelper(IConfiguration config, TelemetryClient telemetry, ClaimsPrincipal user)
  {
    _telemetryClient = telemetry;
    _tableServiceClient = new TableServiceClient(config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"));
    _tableServiceClient.CreateTableIfNotExistsAsync(_coreTableName).GetAwaiter().GetResult();
    _coreTableClient = _tableServiceClient.GetTableClient(_coreTableName);
    _user = user;
    _upn = _user.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
  }

  public List<TablesTableModel> GetToDoLists()
  {
    _telemetryClient.TrackEvent("ListTodoLists");
    _telemetryClient.TrackTrace("TraceMessage GetToDoLists");
    
    return _coreTableClient.Query<TablesTableModel>(filter: $"PartitionKey eq '{_upn}'").ToList<TablesTableModel>();
  }


  public List<TodoModel> GetToDos()
  {
    if (_tableClient == null) {
      //var upn = User.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
      var tables = _coreTableClient.Query<TablesTableModel>(filter: $"PartitionKey eq '{_upn!}'");
      foreach (TablesTableModel m in tables) {
        Console.WriteLine(m.RowKey);
      }
      _tableClient = _tableServiceClient.GetTableClient(tables.First<TablesTableModel>().RowKey);
      _telemetryClient.TrackEvent("ListTodo");
      _telemetryClient.TrackTrace("TraceMessage GetToDos");
      return EnumerateDocumentsAsync(_tableClient);
    }
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

   public async Task<Azure.Response> PostCreateToDoList(String _listName)
  {
    Console.WriteLine(_listName);
    _telemetryClient.TrackEvent("CreateTodoList");
    _telemetryClient.TrackTrace("TraceMessage CreateToDoList");
    return await CreateToDoList(_listName, _upn);
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
      _telemetryClient.TrackEvent($"InsertItem - {result}");
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

  private async Task<Azure.Response> CreateToDoList(string _listName, string upn)
  {
    Azure.Response result;
    try
    {
      await _tableServiceClient.CreateTableIfNotExistsAsync(_listName);
      _tableClient = _tableServiceClient.GetTableClient(_listName);

      var model = new TablesTableModel(_listName, upn);
      result = await _coreTableClient.AddEntityAsync<TablesTableModel>(model);

      return result;
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
  }
}
