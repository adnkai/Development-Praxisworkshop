
namespace Project;

    public class QWorker
    {
        [FunctionName("QWorker")]
        public void Run([QueueTrigger("archiveq", Connection = "STORAGE_CONNECTION_STRING")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
