public interface IUserDataSingleton
{
    String upn {get; set;}
    TableClient tableClient {get; set;}
}

public class UserDataSingleton : IUserDataSingleton
{
    public UserDataSingleton(){}

    public String upn {get; set;}
    public TableClient tableClient {get; set;}
    
}