
namespace Project;
class TableSettings
{
  //Infos we will need
  public string StorageConnectionString { get; }
  public string UPN { get; }
  //public string StorageAccount { get; }
  //public string StorageKey { get; }

  //Access to CoreTable und filter further Requests
  private TableServiceClient _tableServiceClient;
  
  //Current Table of interest
  private TableClient _currentTable { get; set; }
  
  //Core Table where access is defined by upn
  private TableClient _coreTable { get; set; }
  
  //Name of our CoreTable we use
  private string CoreTableName = "TablesTable";

  public TableSettings(string _upn)
  {
    if (string.IsNullOrEmpty(_upn))
    {
      throw new ArgumentNullException("missing upn");
    }
    
    //this.StorageAccount = System.Environment.GetEnvironmentVariable("STORAGE_NAME", EnvironmentVariableTarget.Process);
    this.StorageConnectionString = System.Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING", EnvironmentVariableTarget.Process);
    this.UPN = _upn;

    this._tableServiceClient = new TableServiceClient(this.StorageConnectionString);
    try
    {
      this._coreTable = _tableServiceClient.GetTableClient(this.CoreTableName);
    }
    catch(Exception)
    {
      _tableServiceClient.CreateTableIfNotExists(this.CoreTableName);
      this._coreTable = _tableServiceClient.GetTableClient(this.CoreTableName); 
    }
  }

  public Pageable<CoreTableModel> GetListsForUser()
  {
    //AsyncPageable<CoreTableModel> _pages = this._coreTable.QueryAsync<CoreTableModel>(filter: $"PartitionKey eq '{this.UPN}'");

    //await _pages.ToListAsnyc();
    return this._coreTable.Query<CoreTableModel>(filter: $"PartitionKey eq '{this.UPN}'");
  }

  public void GetTable(string _tableName)
  {
   try
    {
      this._currentTable = this._tableServiceClient.GetTableClient(_tableName);
      Console.WriteLine($"Current Table Success: Table: {_tableName}, UPN: {this.UPN}, {this._currentTable.AccountName}");
    }
    catch(Exception e)
    {
      Console.WriteLine($"Current Table Error: {e.Message}, Table: {_tableName}, UPN: {this.UPN}");
      _tableServiceClient.CreateTableIfNotExists(_tableName);
      this._currentTable = this._tableServiceClient.GetTableClient(_tableName);
      Console.WriteLine($"Current Table Maybe doch: {this._currentTable.AccountName}");
    }
  }

  public async Task<ListElementModel> InsertItem(string _tableName, ListElementModel _listElement)
  {
    if (_listElement == null)
    {
      throw new NullReferenceException("Null Element");
    }

    GetTable(_tableName);
    
    var _response = await this._currentTable.UpsertEntityAsync<ListElementModel>(_listElement);
    
    if(_response.IsError)
    {
      throw new OperationCanceledException(_response.ReasonPhrase);
    }

    return _listElement;
  }

  public async Task<bool> DeleteItem(string _tableName, string _rowKey)
  {
    if (_rowKey == null)
    {
      throw new NullReferenceException("Null Element");
    }

    GetTable(_tableName);

    var _response  = await this._currentTable.DeleteEntityAsync(_tableName, _rowKey);
    
    if(_response.IsError)
    {
      throw new OperationCanceledException(_response.ReasonPhrase);
    }

    return !_response.IsError;
  }

  public async Task<List<ListElementModel>> GetItemsForTable(string _tableName)
  {
    GetTable(_tableName);

    List<ListElementModel> _elements = new List<ListElementModel>();

    await foreach (ListElementModel todo in this._currentTable.QueryAsync<ListElementModel>())
    {
      _elements.Add(todo);
    }

    _elements.Sort((x, y) => x.TaskDescription.CompareTo(y.TaskDescription));

    return _elements;
  }

  public async Task<Dictionary<String, List<ListElementModel>>> GetAllItemsForUser()
  {
    Dictionary<String, List<ListElementModel>> _allListsWithElements = new Dictionary<string, List<ListElementModel>>();

    foreach (var _list in GetListsForUser())
    {
      _allListsWithElements[_list.RowKey] = await GetItemsForTable(_list.RowKey);
    }

    return _allListsWithElements;
  }

  public async Task<ListElementModel> GetItem(string _tableName, string _rowKey)
  {
    GetTable(_tableName);

    return await this._currentTable.GetEntityAsync<ListElementModel>(partitionKey: _tableName, rowKey: _rowKey);
  }

  public async Task<bool> CreateTableForUser(string _tableName)
  {
    var _tableResponse = await this._tableServiceClient.CreateTableIfNotExistsAsync(_tableName);
    
    if (_tableResponse.GetRawResponse().IsError)
    {
      Console.WriteLine($"Create Table Error: {_tableResponse.GetRawResponse().ReasonPhrase}, Table: {_tableName}, UPN: {this.UPN}, ServiceClient: {this._tableServiceClient.AccountName}");
      return false;
    }

    CoreTableModel _newTable = new CoreTableModel(_tableName, this.UPN);
    var response = await this._coreTable.AddEntityAsync<CoreTableModel>(_newTable);
    Console.WriteLine($"Create Table Error: {response.ReasonPhrase}, Table: {_tableName}, UPN: {this.UPN}");
    
    return !response.IsError;
  }

  public async Task<bool> DeleteTableForUser(string _tableName)
  {
    var _response = await this._coreTable.DeleteEntityAsync(this.UPN, _tableName);
    
    if(_response.IsError)
    {
      return false;
    }

    var _tableResponse = await this._tableServiceClient.DeleteTableAsync(_tableName);
    
    return !_tableResponse.IsError;
  }
}