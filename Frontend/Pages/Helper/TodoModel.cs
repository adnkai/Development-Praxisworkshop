using System;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;

namespace Development_Praxisworkshop.Helper
{
  public class TodoModel : TableEntity
  {
    [JsonProperty(PropertyName = "Id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("n");

    [JsonProperty(PropertyName = "CreatedTime")]
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    [JsonProperty(PropertyName = "TaskDescription")]
    public string TaskDescription { get; set; }

    [JsonProperty(PropertyName = "IsCompleted")]
    public bool IsCompleted { get; set; }

    public TodoModel()
    {
      PartitionKey = "TODO";
      RowKey = new Random().Next(0, 9999999) + ":" + new Random().Next(0, 9999999); 
      ETag = "*";
    }

    public TodoModel(string _rowKey, string _partititonKey)
    {
      PartitionKey = _partititonKey ?? "TODO";
      RowKey = _rowKey ?? new Random().Next(0, 9999999) + ":" + new Random().Next(0, 9999999); 
      ETag = "*";
    }
  }
}