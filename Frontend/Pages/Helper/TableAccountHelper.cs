namespace Development_Praxisworkshop.Helper;

public class TableAccountHelper
{
  private TableServiceClient _tableServiceClient;
  private TableClient _coreTableClient;
  private TableClient _tableClient;
  readonly TelemetryClient _telemetryClient;
  private String _upn;
  private Pageable<CoreTableModel> _tables;
  private IConfiguration _config;
  private Dictionary<String, List<ListElementModel>> _todos = new Dictionary<string, List<ListElementModel>>();


  private string _coreTableName = "TablesTable";

  public TableAccountHelper(IConfiguration config, TelemetryClient telemetry, ClaimsPrincipal user)
  {
    _config = config;
    _telemetryClient = telemetry;
    _tableServiceClient = new TableServiceClient(_config.GetValue<String>("StorageAccount:StorageConnectionString2"));
    // Create initial Tables-Table
    _ = _tableServiceClient.CreateTableIfNotExistsAsync(_coreTableName).GetAwaiter().GetResult();
    _coreTableClient = _tableServiceClient.GetTableClient(_coreTableName);

    _upn = user.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
    _tables = _coreTableClient.Query<CoreTableModel>(filter: $"PartitionKey eq '{_upn!}'");
  }

  public List<CoreTableModel> GetToDoLists()
  {
    _telemetryClient.TrackEvent("GetToDoLists");
    _telemetryClient.TrackTrace("TraceMessage GetToDoLists");
    
    return _coreTableClient.Query<CoreTableModel>(filter: $"PartitionKey eq '{_upn}'").ToList<CoreTableModel>();
  }


  public async Task<Dictionary<String, List<ListElementModel>>> GetToDos()
  {
    if (_tables.Count() > 0) {
      foreach(var table in _tables) {
        _tableClient = _tableServiceClient.GetTableClient(table.RowKey);
        _todos[table.RowKey] = await EnumerateDocumentsAsync(_tableClient);
      }
    }

    _telemetryClient.TrackEvent("GetToDos");
    _telemetryClient.TrackTrace("TraceMessage GetToDos");

    return _todos;
  }

  private async Task<List<ListElementModel>> EnumerateDocumentsAsync(TableClient _tableClient)
  {
    List<ListElementModel> tmpTodos = new List<ListElementModel>();
    await foreach (ListElementModel todo in _tableClient.QueryAsync<ListElementModel>())
    {
      tmpTodos.Add(todo);
    }
    tmpTodos.Sort((x, y) => x.TaskDescription.CompareTo(y.TaskDescription));
    _telemetryClient.TrackMetric("CUSTOM_ListTodo_Count", tmpTodos.Count);
    return tmpTodos;
  }

  public async Task<ListElementModel> PostToDo(ListElementModel _todoItem, String listName)
  {
    _tableClient = _tableServiceClient.GetTableClient(listName);
    try
    {
      Azure.Response result;
      result = await _tableClient.AddEntityAsync<ListElementModel>(_todoItem);
      _telemetryClient.TrackEvent($"InsertItem - {result}");
      return _todoItem;
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
    
  }

  public async Task<ListElementModel> MarkDoneToDo(string rowKey, string tableName)
  {
    _tableClient = _tableServiceClient.GetTableClient(tableName);
    Pageable<ListElementModel> queryResultsFilter = _tableClient.Query<ListElementModel>(filter: $"RowKey eq '{rowKey}'");

    Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");

    // ListElementModel updatedToDo;
    Azure.Response response;
    ListElementModel m;
    try
    {
      ListElementModel item = queryResultsFilter.First();

      if (item == null)
      {
        throw new NullReferenceException();
      }

      m = item;
      m.IsCompleted = m.IsCompleted ? false : true;
      response = await _tableClient.UpdateEntityAsync<ListElementModel>(m, ETag.All, TableUpdateMode.Merge);
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
    _telemetryClient.TrackMetric("CUSTOM_MarkedTodo_Value", m.IsCompleted == false ? 0 : 1);
    return m;
  }

  public async Task<Azure.Response> DeleteToDo(string rowKey, string listName)
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

  

  public async Task<Azure.Response> PostCreateToDoList(string listName)
  {
    Azure.Response result;
    try
    {
      await _tableServiceClient.CreateTableIfNotExistsAsync(listName);
      // _tableClient = _tableServiceClient.GetTableClient(_listName);

      var model = new CoreTableModel(listName, _upn);
      result = await _coreTableClient.AddEntityAsync<CoreTableModel>(model);

      return result;
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
  }

  public async Task<Azure.Response> PostDeleteToDoList(string listName)
  {
    Azure.Response result;
    try
    {
      result = await _tableServiceClient.DeleteTableAsync(listName);
      result = await _coreTableClient.DeleteEntityAsync(_upn, listName);
      
      return result;
    }
    catch (System.Exception e)
    {
      System.Console.WriteLine(e.StackTrace);
      throw;
    }
  }

  // Queue Storage
  public async Task<Azure.Response> ArchiveList(string listName)
  {
    // Queue Zeug
    var qClient = new QueueClient(_config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"), "archiveq");
    _ = await qClient.CreateIfNotExistsAsync();
    
    _tableClient = _tableServiceClient.GetTableClient(listName);
    List<ListElementModel> todos = await EnumerateDocumentsAsync(_tableClient);

    var batch = new List<Task>();
    foreach (ListElementModel item in todos)
    {
      batch.Add(qClient.SendMessageAsync(item.TaskDescription));
    }
    await Task.WhenAll(batch);
    return null;
  }


}



