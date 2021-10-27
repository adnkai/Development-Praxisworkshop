using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Table;

namespace Project
{
  class TableSettings
  {
    public string StorageAccount { get; }
    public string StorageKey { get; }
    public string TableName { get; }

    private CloudTable _table { get; set; }

    public TableSettings(string tableName)
    {
      
      if (string.IsNullOrEmpty(tableName))
      {
        throw new ArgumentNullException("TableName");
      }
      
      this.StorageAccount = System.Environment.GetEnvironmentVariable("STORAGE_NAME", EnvironmentVariableTarget.Process);
      this.StorageKey = System.Environment.GetEnvironmentVariable("STAORAGE_KEY", EnvironmentVariableTarget.Process);
      this.TableName = tableName;
    }

    private async Task GetTableAsync()
    {
      //Account
      CloudStorageAccount storageAccount = new CloudStorageAccount(
          new StorageCredentials(this.StorageAccount, this.StorageKey), true);

      //Client
      CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

      //Table
      CloudTable table = tableClient.GetTableReference(this.TableName);
      await table.CreateIfNotExistsAsync();

      this._table = table;
    }

    public async Task<Todo> InsertItem(Todo _todoItem)
    {
      if (_todoItem == null)
      {
        throw new NullReferenceException();
      }

      if (this._table == null)
      {
        await GetTableAsync();
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

      Todo newTodo = (Todo)result.Result;

      return newTodo;
    }

    public async Task<Todo> DeleteItem(Todo _todoItem)
    {
      if (_todoItem == null)
      {
        throw new NullReferenceException();
      }

      if (this._table == null)
      {
        await GetTableAsync();
      }

      TableResult result;
      TableOperation operation = TableOperation.Delete(_todoItem);

      try
      {
        result = await _table.ExecuteAsync(operation);
      }
      catch (System.Exception e)
      {
        System.Console.WriteLine(e.StackTrace);
        throw;
      }

      Todo deletedTodo = (Todo)result.Result;

      return deletedTodo;
    }

    public async Task<List<Todo>> GetItems()
    {
      List<Todo> todos = new List<Todo>();

      if (this._table == null)
      {
        await GetTableAsync();
      }

      TableQuery<Todo> query = new TableQuery<Todo>();


      TableResult result;

      try
      {
        foreach (Todo t in _table.ExecuteQuery(query))
        {
          todos.Add(t);
        }
      }
      catch (System.Exception e)
      {
        System.Console.WriteLine(e.StackTrace);
        throw;
      }

      return todos;
    }

    public async Task<Todo> GetItem(string _pKey, string _id)
    {
      Todo todo;

      if (this._table == null)
      {
        await GetTableAsync();
      }

      TableOperation operation = TableOperation.Retrieve<Todo>(_pKey, _id);

      TableResult result;

      try
      {
        result = await _table.ExecuteAsync(operation);
        todo = (Todo)result.Result;
      }
      catch (System.Exception e)
      {
        System.Console.WriteLine(e.StackTrace);
        throw;
      }

      return todo;
    }
  }
}
