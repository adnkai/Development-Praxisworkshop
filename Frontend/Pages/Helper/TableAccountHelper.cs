namespace Development_Praxisworkshop.Helper;

public class TableAccountHelper
{
  private TableServiceClient _tableServiceClient;
  private TableClient _coreTableClient;
  private TableClient _tableClient;
  readonly TelemetryClient _telemetryClient;
  private ClaimsPrincipal _user;
  private String _upn;
  private Pageable<TablesTableModel> _tables;
  private IConfiguration _config;
  public Dictionary<String, List<TodoModel>> _todos;


  private string _coreTableName = "TablesTable";

  public TableAccountHelper(IConfiguration config, TelemetryClient telemetry, ClaimsPrincipal user)
  {
    _config = config;
    _telemetryClient = telemetry;
    _tableServiceClient = new TableServiceClient(_config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"));
    _tableServiceClient.CreateTableIfNotExistsAsync(_coreTableName).GetAwaiter().GetResult();
    _coreTableClient = _tableServiceClient.GetTableClient(_coreTableName);
    _user = user;
    _upn = _user.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;

    _tables = _coreTableClient.Query<TablesTableModel>(filter: $"PartitionKey eq '{_upn!}'");
    _todos = new Dictionary<string, List<TodoModel>>();

    if (_tables.Count() > 0) {
      _tableClient = _tableServiceClient.GetTableClient(_tables.First<TablesTableModel>().RowKey);
    }
  }

  public List<TablesTableModel> GetToDoLists()
  {
    _telemetryClient.TrackEvent("ListTodoLists");
    _telemetryClient.TrackTrace("TraceMessage GetToDoLists");
    
    return _coreTableClient.Query<TablesTableModel>(filter: $"PartitionKey eq '{_upn}'").ToList<TablesTableModel>();
  }


  public async Task<Dictionary<String, List<TodoModel>>> GetToDos()
  {
    var tmpList = new List<TodoModel>();
    if (_tableClient == null) {
      return new Dictionary<String,List<TodoModel>>();
    }

    if (_tables.Count() > 0) {
      foreach(var table in _tables) {
        var tableClient = _tableServiceClient.GetTableClient(table.RowKey);
        _todos[table.RowKey] = await EnumerateDocumentsAsync(tableClient);
      }
    }

    _telemetryClient.TrackEvent("ListTodo");
    _telemetryClient.TrackTrace("TraceMessage GetToDos");

    // return await EnumerateDocumentsAsync(_tableClient);
    return _todos;
  }

  public async Task<TodoModel> PostToDo(TodoModel _todo, string listName)
  {
    _telemetryClient.TrackEvent("CreateTodo");
    _telemetryClient.TrackMetric("CUSTOM_CreatedTodo_DescriptionLength", _todo.TaskDescription.Length);
    _telemetryClient.TrackTrace("TraceMessage CreateToDos");
    return await InsertItem(_todo, listName);
  }

   public async Task<Azure.Response> PostCreateToDoList(String _listName)
  {
    _telemetryClient.TrackEvent("CreateTodoList");
    _telemetryClient.TrackTrace("TraceMessage CreateToDoList");
    return await CreateToDoList(_listName, _upn);
  }

  public async Task<Azure.Response> PostDeleteToDoList(String _listName)
  {
    _telemetryClient.TrackEvent("DeleteTodoList");
    return await DeleteToDoList(_listName, _upn);
  }

  public async Task<TodoModel> MarkDoneToDo(string rowKey, string tableName)
  {
    _telemetryClient.TrackEvent("MarkDoneTodo");
    return await UpdateToDo(rowKey, tableName);
  }

  public async Task<Azure.Response> DeleteToDo(string rowKey, string listName)
  {
    _telemetryClient.TrackEvent("DeleteTodo");
    _telemetryClient.TrackTrace("TraceMessage DeleteToDos");
    _telemetryClient.TrackException(new OverflowException());
    return await DelToDo(rowKey, listName);
  }

  public async Task<Azure.Response> ArchiveList(string listName)
  {
    _telemetryClient.TrackEvent("DeleteTodo");
    _telemetryClient.TrackTrace("TraceMessage DeleteToDos");
    _telemetryClient.TrackException(new OverflowException());
    return await Archive(listName);
  }

  private async Task<List<TodoModel>> EnumerateDocumentsAsync(TableClient _tableClient)
  {
    List<TodoModel> tmpTodos = new List<TodoModel>();
    await foreach (TodoModel todo in _tableClient.QueryAsync<TodoModel>())
    {
      tmpTodos.Add(todo);
    }
    tmpTodos.Sort((x, y) => x.TaskDescription.CompareTo(y.TaskDescription));
    _telemetryClient.TrackMetric("CUSTOM_ListTodo_Count", tmpTodos.Count);
    return tmpTodos;
  }

  private async Task<TodoModel> InsertItem(TodoModel _todoItem, String listName)
  {
    if (_todoItem == null)
    {
      throw new NullReferenceException();
    }
    _tableClient = _tableServiceClient.GetTableClient(listName);
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

  private async Task<TodoModel> UpdateToDo(string rowKey, string tableName)
  {
    _tableClient = _tableServiceClient.GetTableClient(tableName);
    Pageable<TodoModel> queryResultsFilter = _tableClient.Query<TodoModel>(filter: $"RowKey eq '{rowKey}'");

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

  private async Task<Azure.Response> DelToDo(string rowKey, string listName)
  {
    _tableClient = _tableServiceClient.GetTableClient(listName);
    Azure.Response result;
    try
    {
      result = await _tableClient.DeleteEntityAsync(listName, rowKey);
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
    return result;
  }

  private async Task<Azure.Response> Archive(string listName)
  {
    // Queue Zeug
    var qClient = new QueueClient(_config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"), "archiveq");
    _ = await qClient.CreateIfNotExistsAsync();
    
    _tableClient = _tableServiceClient.GetTableClient(listName);
    List<TodoModel> todos = await EnumerateDocumentsAsync(_tableClient);

    var batch = new List<Task>();
    foreach (TodoModel item in todos)
    {
      batch.Add(qClient.SendMessageAsync(item.TaskDescription));
    }
    await Task.WhenAll(batch);
    return null;
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

  private async Task<Azure.Response> DeleteToDoList(string _listName, string upn)
  {
    Azure.Response result;
    try
    {
      result = await _tableServiceClient.DeleteTableAsync(_listName);
      result = await _coreTableClient.DeleteEntityAsync(upn, _listName);
      
      return result;
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
  }
}
