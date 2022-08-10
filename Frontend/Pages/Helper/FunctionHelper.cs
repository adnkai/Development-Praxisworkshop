namespace Development_Praxisworkshop.Helper;
public class FunctionHelper
{
  private readonly HttpClient client = new HttpClient();
  private readonly TelemetryClient _telemetryClient;

  public FunctionHelper(IConfiguration config, TelemetryClient telemetryClient)
  {
    _telemetryClient = telemetryClient;

    client.BaseAddress = new Uri(config.GetSection("Function").GetValue<string>("FunctionUri"));

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("x-functions-key", config.GetSection("Function").GetValue<string>("DefaultHostKey"));
  }

  public async Task<List<ListElementModel>> GetToDos()
  {
    _telemetryClient.TrackEvent("ListTodoFunction");
    return await EnumerateDocumentsAsync();
  }

  public async Task<string> PostToDo(string _todo)
  {
    _telemetryClient.TrackEvent("CreateTodoFunction");
    return await InsertItem(_todo);
  }

  public async Task<string> MarkDoneToDo(string _rowKey)
  {
    _telemetryClient.TrackEvent("MarkDoneTodoFunction");
    return await UpdateToDo(_rowKey);
  }

  public async Task<string> DeleteToDo(string _rowKey)
  {
    _telemetryClient.TrackEvent("DeleteTodoFunction");
    return await DelToDo(_rowKey);
  }

  private async Task<List<ListElementModel>> EnumerateDocumentsAsync()
  {
    List<ListElementModel> tmpTodos = new List<ListElementModel>();

    try
    {
      var response = await client.GetAsync("getTodos");

      if (!response.IsSuccessStatusCode)
      {
        System.Console.WriteLine("Unnssuccessffull");
        System.Console.WriteLine(response.StatusCode);

        return tmpTodos;
      }

      var data = await response.Content.ReadAsStringAsync();
      tmpTodos = JsonConvert.DeserializeObject<List<ListElementModel>>(data)!;
    }
    catch { }

    return tmpTodos!;
  }

  private async Task<string> InsertItem(string _todoTask)
  {
    if (String.IsNullOrEmpty(_todoTask))
    {
      throw new NullReferenceException();
    }

    var json = JsonConvert.SerializeObject(
        new Dictionary<string, string>
        {
          { "TaskDescription", _todoTask }
        }
    );
    var content = new StringContent(json);

    var response = await client.PostAsync("postTodo", content);

    if (!response.IsSuccessStatusCode)
    {
      System.Console.WriteLine("Unnssuccessfull");
      System.Console.WriteLine(response.StatusCode);

      return await Task.FromResult<string>("Unnssuccessfull");
    }

    return await response.Content.ReadAsStringAsync();
  }

  private async Task<string> UpdateToDo(string _rowKey)
  {
    if (String.IsNullOrEmpty(_rowKey))
    {
      throw new NullReferenceException();
    }
    
    var tData = await client.GetAsync($"getTodo/{_rowKey}");

    if(!tData.IsSuccessStatusCode)
    {
      System.Console.WriteLine("Unssuccessfull");
      System.Console.WriteLine(tData.StatusCode);

      return await Task.FromResult<string>("Unssuccessfull");
    }
    
    ListElementModel toUpdate = JsonConvert.DeserializeObject<ListElementModel>(await tData.Content.ReadAsStringAsync())!;
    toUpdate.IsCompleted = toUpdate.IsCompleted ? false : true;

    var content = new StringContent(JsonConvert.SerializeObject(toUpdate));

    var response = await client.PostAsync("postTodo", content);

    if (!response.IsSuccessStatusCode)
    {
      System.Console.WriteLine("Unssuccessfull");
      System.Console.WriteLine(response.StatusCode);

      return await Task.FromResult<string>("Unssuccessfull");
    }

    return await response.Content.ReadAsStringAsync();
  }

  private async Task<string> DelToDo(string _rowKey)
  {
      var json = JsonConvert.SerializeObject(
        new Dictionary<string, string>
        {
          { "RowKey", _rowKey }
        }
    );

    var request = new HttpRequestMessage {
          Method = HttpMethod.Delete,
          RequestUri = new Uri($"{client.BaseAddress}deleteTodo"),
          Content = new StringContent(json)
    };

    var response = await client.SendAsync(request);

    if (!response.IsSuccessStatusCode)
    {
      System.Console.WriteLine("Unssuccessfull");
      System.Console.WriteLine(response.StatusCode);

      return await Task.FromResult<string>("Unssuccessfull");
    }

    return await response.Content.ReadAsStringAsync();
  }
}

