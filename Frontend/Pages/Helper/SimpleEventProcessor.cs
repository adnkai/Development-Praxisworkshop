namespace Development_Praxisworkshop;


#region EventProcessorInterface
public class SimpleEventProcessor : IEventProcessor
{
    private readonly HttpClient client = new HttpClient();

    public SimpleEventProcessor() {
        client.BaseAddress = new Uri("https://devworkshop.azurewebsites.net/");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("aeg-event-type","Notification");
    }

    public Task CloseAsync(PartitionContext context, CloseReason reason)
    {
        Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
        _ = SendPostReq("CloseAsync").Result;
        return Task.CompletedTask;
    }

    public Task OpenAsync(PartitionContext context)
    {
        Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
        _ = SendPostReq("OpenAsync").Result;
        return Task.CompletedTask;
    }

    public Task ProcessErrorAsync(PartitionContext context, Exception error)
    {
        Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
        _ = SendPostReq("ProcessErrorAsync").Result;
        return Task.CompletedTask;
    }

    public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
    {
        foreach (var eventData in messages)
        {
            var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
            Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");
            _ = SendPostReq(data).Result;
        }
        
        return context.CheckpointAsync();
    }

    private async Task<String> SendPostReq(dynamic data) {
        var json = JsonConvert.SerializeObject(
            data
        );
        var content = new StringContent(data);
        var response = await client.PostAsync($"EventsUpdates", content);

        if (!response.IsSuccessStatusCode)
        {
        System.Console.WriteLine("Unnssuccessfull");
        System.Console.WriteLine(response.StatusCode);

        return await Task.FromResult<string>("Unnssuccessfull");
        }

        return await response.Content.ReadAsStringAsync();
    }
}
#endregion