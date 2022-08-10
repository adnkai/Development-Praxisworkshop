

namespace Project;

public class Todo : TableEntity
{
  [JsonProperty(PropertyName = "Id")]
  public string Id { get; set; } = Guid.NewGuid().ToString("n");

  [JsonProperty(PropertyName = "CreatedTime")]
  public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

  [JsonProperty(PropertyName = "TaskDescription")]
  public string TaskDescription { get; set; }

  [JsonProperty(PropertyName = "IsCompleted")]
  public bool IsCompleted { get; set; }

  public Todo()
  {
    PartitionKey = "TODO";
    RowKey = new Random().Next(0, 9999999) + ":" + new Random().Next(0, 9999999); ;
    ETag = "*";
  }

  public Todo(string _rowKey, string _partititonKey)
  {
    PartitionKey = _partititonKey ?? "TODO";
    RowKey = _rowKey ?? new Random().Next(0, 9999999) + ":" + new Random().Next(0, 9999999); ;
    ETag = "*";
  }
}