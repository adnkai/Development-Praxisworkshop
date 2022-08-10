namespace Development_Praxisworkshop.Pages;

[AllowAnonymous]
[IgnoreAntiforgeryToken(Order = 1001)]


public class EventsUpdatesModel : PageModel
{
    
    private bool EventTypeSubcriptionValidation
        => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
            "SubscriptionValidation";

    private bool EventTypeNotification
        => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
            "Notification";

    private readonly IHubContext<GridEventsHub> _hubContext;
    
    public EventsUpdatesModel(IHubContext<GridEventsHub> gridEventsHubContext, ILogger<PrivacyModel> logger, IConfiguration config, TelemetryClient telemetryClient)
    {
        this._hubContext = gridEventsHubContext;
    }

    
    public async Task<IActionResult> OnPost()
    {
        Console.WriteLine("POSTED EVENT");
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            var jsonContent = await reader.ReadToEndAsync();
            Console.WriteLine(jsonContent);

            // Check the event type.
            // Return the validation code if it's 
            // a subscription validation request. 
            if (EventTypeSubcriptionValidation)
            {
                return await HandleValidation(jsonContent);
            }
            else if (EventTypeNotification)
            {
                // Check to see if this is passed in using
                // the CloudEvents schema
                if (IsCloudEvent(jsonContent))
                {
                    return await HandleCloudEvent(jsonContent);
                }

                return await HandleGridEvents(jsonContent);
            }

            return BadRequest();                
        }
    }

    private async Task<JsonResult> HandleValidation(string jsonContent)
    {
        var gridEvent =
            JsonConvert.DeserializeObject<List<GridEvent<Dictionary<string, string>>>>(jsonContent)
                .First();

        await this._hubContext.Clients.All.SendAsync(
            "gridupdate",
            gridEvent.Id,
            gridEvent.EventType,
            gridEvent.Subject,
            gridEvent.EventTime.ToLongTimeString(),
            jsonContent.ToString());

        // Retrieve the validation code and echo back.
        var validationCode = gridEvent.Data["validationCode"];
        return new JsonResult(new
        {
            validationResponse = validationCode
        });
    }

    private async Task<IActionResult> HandleGridEvents(string jsonContent)
    {
        var events = JArray.Parse(jsonContent);
        foreach (var e in events)
        {
            // Invoke a method on the clients for 
            // an event grid notiification.                        
            var details = JsonConvert.DeserializeObject<GridEvent<dynamic>>(e.ToString());
            await this._hubContext.Clients.All.SendAsync(
                "gridupdate",
                details.Id,
                details.EventType,
                details.Subject,
                details.EventTime.ToLongTimeString(),
                e.ToString());
        }

        return new EmptyResult();
    }

    private async Task<IActionResult> HandleCloudEvent(string jsonContent)
    {
        var details = JsonConvert.DeserializeObject<CloudEvent<dynamic>>(jsonContent);
        var eventData = JObject.Parse(jsonContent);

        await this._hubContext.Clients.All.SendAsync(
            "gridupdate",
            details.Id,
            details.Type,
            details.Subject,
            details.Time,
            eventData.ToString()
        );

        return new EmptyResult();
    }

    private static bool IsCloudEvent(string jsonContent)
    {
        // Cloud events are sent one at a time, while Grid events
        // are sent in an array. As a result, the JObject.Parse will 
        // fail for Grid events. 
        try
        {
            // Attempt to read one JSON object. 
            var eventData = JObject.Parse(jsonContent);

            // Check for the spec version property.
            var version = eventData["specversion"].Value<string>();
            if (!string.IsNullOrEmpty(version)) return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }
}