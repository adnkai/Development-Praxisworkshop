namespace Project;

public class ListElementModel : ITableEntity
{
  [JsonProperty(PropertyName = "PartitionKey")]
  public string PartitionKey { get; set; } = Guid.NewGuid().ToString("n");

  [JsonProperty(PropertyName = "RowKey")]
  public string RowKey { get; set; } = Guid.NewGuid().ToString("n");

  [JsonProperty(PropertyName = "ETag")]
  public ETag ETag { get; set; } = new ETag();

  [JsonProperty(PropertyName = "Id")]
  public string Id { get; set; } = Guid.NewGuid().ToString("n");

  [JsonProperty(PropertyName = "Timestamp")]
  public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.Now;

  [JsonProperty(PropertyName = "CreatedTime")]
  public DateTime? CreatedTime { get; set; } = DateTime.UtcNow;

  [JsonProperty(PropertyName = "TaskDescription")]
  public string TaskDescription { get; set; }

  [JsonProperty(PropertyName = "IsCompleted")]
  public bool IsCompleted { get; set; }

  public ListElementModel()
  {
    PartitionKey = "ListElementModel";
    RowKey = new Random().Next(0, 9999999) + ":" + new Random().Next(0, 9999999); ;
    ETag = ETag.All;
    TaskDescription = "";
  }

  public ListElementModel(string _rowKey, string _partititonKey)
  {
    PartitionKey = _partititonKey ?? "ListElementModel";
    RowKey = _rowKey ?? new Random().Next(0, 9999999) + ":" + new Random().Next(0, 9999999); ;
    ETag = ETag.All;
    TaskDescription = "";
  }
}