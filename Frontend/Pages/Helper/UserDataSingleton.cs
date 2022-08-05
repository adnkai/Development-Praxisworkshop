public interface IDataSingleton
{
    String upn {get; set;}
    TableClient tableClient {get; set;}
}

public class DataSingleton : IDataSingleton
{
    public DataSingleton(){}

    public String upn {get; set;}
    public TableClient tableClient {get; set;}
    
}