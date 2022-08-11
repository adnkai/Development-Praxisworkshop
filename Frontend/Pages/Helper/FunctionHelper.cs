namespace Development_Praxisworkshop.Helper;
public class FunctionHelper
{
  private readonly HttpClient client = new HttpClient();
  private readonly TelemetryClient _telemetryClient;
  private String _upn;

  #region constructor
  public FunctionHelper(IConfiguration config, TelemetryClient telemetryClient, ClaimsPrincipal user)
  {
    _telemetryClient = telemetryClient;
    _upn = user.Claims?.FirstOrDefault(x => x.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))?.Value;
    
    client.BaseAddress = new Uri(config.GetSection("Function").GetValue<string>("FunctionUri"));

    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("x-functions-key", config.GetSection("Function").GetValue<string>("DefaultHostKey"));
  }
  #endregion

  #region GetTodos
  public async Task<Dictionary<String, List<ListElementModel>>> GetToDos()
  {
    _telemetryClient.TrackEvent("ListTodoFunction");
    
    Dictionary<String, List<ListElementModel>> tmpTodos = new Dictionary<String, List<ListElementModel>>();

    try
    {
      var response = await client.GetAsync($"getAllElementsForUser?upn={_upn}");

      if (!response.IsSuccessStatusCode)
      {
        System.Console.WriteLine("Unnssuccessffull");
        System.Console.WriteLine(response.StatusCode);

        return tmpTodos;
      }

      var data = await response.Content.ReadAsStringAsync();
      tmpTodos = JsonConvert.DeserializeObject<Dictionary<String, List<ListElementModel>>>(data)!;
    }
    catch { }

    return tmpTodos!;

  }
  #endregion
 
  #region Post Todos
  public async Task<string> PostToDo(string todoTask, string listName)
  {
    if (String.IsNullOrEmpty(todoTask))
    {
      throw new NullReferenceException();
    }

    var json = JsonConvert.SerializeObject(
      new Dictionary<string, string>
      {
        { "TaskDescription", todoTask},
        { "ListName", listName}
      }
    );
    var content = new StringContent(json);

    var response = await client.PostAsync($"postElement/{listName}", content);

    if (!response.IsSuccessStatusCode)
    {
      System.Console.WriteLine("Unnssuccessfull");
      System.Console.WriteLine(response.StatusCode);

      return await Task.FromResult<string>("Unnssuccessfull");
    }

    return await response.Content.ReadAsStringAsync();
  }  
  #endregion

  #region Update Todos (Mark Done)
    public async Task<string> Update(string rowKey, string listName)
    {
      var tData = await client.GetAsync($"getSingleElementInListForUser/{listName}/{rowKey}/?upn={_upn}");

      if(!tData.IsSuccessStatusCode)
      {
        System.Console.WriteLine("Unssuccessfull");
        System.Console.WriteLine(tData.StatusCode);

        return await Task.FromResult<string>("Unssuccessfull");
      }
      
      ListElementModel toUpdate = JsonConvert.DeserializeObject<ListElementModel>(await tData.Content.ReadAsStringAsync())!;
      // To be refactored for more options
      toUpdate.IsCompleted = toUpdate.IsCompleted ? false : true;

      var content = new StringContent(JsonConvert.SerializeObject(toUpdate));

      var response = await client.PostAsync($"postElement/{listName}?upn={_upn}", content);

      if (!response.IsSuccessStatusCode)
      {
        System.Console.WriteLine("Unssuccessfull");
        System.Console.WriteLine(response.StatusCode);

        return await Task.FromResult<string>("Unssuccessfull");
      }

      return await response.Content.ReadAsStringAsync();
    }
  #endregion

  #region Delete
    public async Task<string> DeleteToDo(string rowKey, string listName)
    {
      var json = JsonConvert.SerializeObject(
        new Dictionary<string, string>
        {
          { "RowKey", rowKey },
          { "ListName", listName }
        }
      );

      var request = new HttpRequestMessage {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{client.BaseAddress}deleteElement/{listName}/{rowKey}?upn={_upn}")
            // ,Content = new StringContent(json)
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
  #endregion

  public async Task<string> CreateToDoList(string listName)
  {

    var json = JsonConvert.SerializeObject(
      new Dictionary<string, string>
      {
        { "ListName", listName}
      }
    );
    var content = new StringContent(json);

    var response = await client.PostAsync($"postElement/{listName}?upn={_upn}", content);

    if (!response.IsSuccessStatusCode)
    {
      System.Console.WriteLine("Unnssuccessfull");
      System.Console.WriteLine(response.StatusCode);

      return await Task.FromResult<string>("Unnssuccessfull");
    }

    return await response.Content.ReadAsStringAsync();
  }

  public async Task<String> DeleteToDoList(string listName)
  {
    var json = JsonConvert.SerializeObject(
      new Dictionary<string, string>
        {
          { "ListName", listName }
        }
      );

      var request = new HttpRequestMessage {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{client.BaseAddress}deleteTable/{listName}?upn={_upn}")
            // ,Content = new StringContent(json)
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

/* FUNCTION STUFF

getAllElementInListForUser: [GET] http://localhost:7071/api/getElements/{tableName}
getSingleElementInListForUser: [GET] http://localhost:7071/api/getElements/{tableName}/{id}
postElement: [POST] http://localhost:7071/api/postElement/{tableName}
postTable: [POST] http://localhost:7071/api/postTable/{tableName}
deleteElement: [DELETE] http://localhost:7071/api/deleteElement/{tableName}/{id}
deleteTable: [DELETE] http://localhost:7071/api/deleteTable/{tableName}

*/