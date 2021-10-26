using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using Microsoft.Extensions.Configuration;

namespace Development_Praxisworkshop.Helper
{
  public class FunctionHelper
  {

    private readonly HttpClient client = new HttpClient();

    public FunctionHelper(IConfiguration config)
    {
      //config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString");
      client.BaseAddress = new Uri(config.GetSection("Function").GetValue<string>("FunctionUri"));

      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      client.DefaultRequestHeaders.Add("x-functions-key", config.GetSection("Function").GetValue<string>("DefaultHostKey"));
    }

    public async Task<List<TodoModel>> GetToDos()
    {
      return await EnumerateDocumentsAsync();
    }

    public async Task<string> PostToDo(string _todo)
    {
      return await InsertItem(_todo);
    }

    public async Task<string> MarkDoneToDo(string _rowKey)
    {
      return await UpdateToDo(_rowKey);
    }

    public async Task<string> DeleteToDo(string _rowKey)
    {
      return await DelToDo(_rowKey);
    }

    private async Task<List<TodoModel>> EnumerateDocumentsAsync()
    {
      List<TodoModel> tmpTodos = new List<TodoModel>();

      try
      {
        var response = await client.GetAsync("getTodos");

        if (!response.IsSuccessStatusCode)
        {
          System.Console.WriteLine("Unnssuccessffull");
          System.Console.WriteLine(response.StatusCode);
          System.Console.WriteLine(client.BaseAddress);
          System.Console.WriteLine(client.DefaultRequestHeaders);

          return tmpTodos;
        }

        var data = await response.Content.ReadAsStringAsync();
        tmpTodos = JsonConvert.DeserializeObject<List<TodoModel>>(data);
      }
      catch { }

      return tmpTodos;
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
        System.Console.WriteLine("Unnssuccessffull");
        System.Console.WriteLine(response.StatusCode);
        System.Console.WriteLine(client.BaseAddress);
        System.Console.WriteLine(client.DefaultRequestHeaders);
      }

      return await response.Content.ReadAsStringAsync();
    }

    private async Task<string> UpdateToDo(string _rowKey)
    {
      var response = await client.GetAsync("/getTodos");

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
            RequestUri = new Uri("deleteTodo"),
            Content = new StringContent(json)
        };

      var response = await client.SendAsync(request);

      if (!response.IsSuccessStatusCode)
      {
        System.Console.WriteLine("Unnssuccessffull");
        System.Console.WriteLine(response.StatusCode);
        System.Console.WriteLine(client.BaseAddress);
        System.Console.WriteLine(client.DefaultRequestHeaders);
      }

      return await response.Content.ReadAsStringAsync();
    }
  }
}
