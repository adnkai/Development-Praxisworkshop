namespace Development_Praxisworkshop.Helper;

public class CoreTableModel : ITableEntity
{
  [JsonProperty(PropertyName = "PartitionKey")] // UPN
  public string PartitionKey { get; set; } = Guid.NewGuid().ToString("n");

  [JsonProperty(PropertyName = "RowKey")] // TableName
  public string RowKey { get; set; } = Guid.NewGuid().ToString("n");

  [JsonProperty(PropertyName = "ETag")]
  public ETag ETag { get; set; } = new ETag();

  [JsonProperty(PropertyName = "Id")]
  public string Id { get; set; } = Guid.NewGuid().ToString("n");

  [JsonProperty(PropertyName = "Timestamp")]
  public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.Now;

  public CoreTableModel()
  {
    PartitionKey = "";
    RowKey = new Random().Next(0, 9999999) + ":" + new Random().Next(0, 9999999); 
    ETag = ETag.All;
  }

  public CoreTableModel(string _rowKey, string _partititonKey)
  {
    PartitionKey = _partititonKey!;
    RowKey = _rowKey ?? new Random().Next(0, 9999999) + ":" + new Random().Next(0, 9999999); 
    ETag = ETag.All;
  }
}