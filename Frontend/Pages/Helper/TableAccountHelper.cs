using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Azure.Storage;
using Microsoft.Azure.Cosmos.Table;

using Microsoft.Extensions.Configuration;

namespace Development_Praxisworkshop.Helper
{
  public class TableAccountHelper
  {
    private CloudTableClient _tableClient;
    private CloudTable _table;

    public TableAccountHelper(IConfiguration config)
    {
      CloudStorageAccount _acc = CloudStorageAccount.Parse(config.GetSection("StorageAccount").GetValue<string>("StorageConnectionString"));
      _tableClient = _acc.CreateCloudTableClient();

      CloudTable table = _tableClient.GetTableReference(config.GetSection("StorageAccount").GetValue<string>("TablePartitionKey"));
      table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
      _table = table;
    }

    public async Task<List<TodoModel>> GetToDos()
    {
      return await EnumerateDocumentsAsync(_table);
    }

    public async Task<TodoModel> PostToDo(TodoModel _todo)
    {
      return await InsertItem(_todo);
    }

    public async Task<TodoModel> MarkDoneToDo(string _rowKey)
    {
      return await UpdateToDo(_rowKey);
    }

    private async Task<List<TodoModel>> EnumerateDocumentsAsync(CloudTable _table)
    {
      List<TodoModel> tmpTodos = new List<TodoModel>();
      TableQuery<TodoModel> query = new TableQuery<TodoModel>();

      foreach (TodoModel todo in _table.ExecuteQuery(query))
      {
        tmpTodos.Add(todo);
      }

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
        m.IsCompleted = true;

        updatedToDo = await InsertItem(m);
      }
      catch (System.Exception e)
      {
        System.Console.WriteLine(e.StackTrace);
        throw;
      }

      return updatedToDo;
    }
  }
}
