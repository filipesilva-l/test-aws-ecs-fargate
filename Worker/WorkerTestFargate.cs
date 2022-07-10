namespace Worker;

public class WorkerTestFargate : BackgroundService
{
    private readonly ILogger<WorkerTestFargate> _logger;
    private readonly SqsClient _sqsClient;
    private readonly IConfiguration _configuration;

    public WorkerTestFargate(ILogger<WorkerTestFargate> logger, SqsClient sqsClient, IConfiguration configuration)
    {
        _logger = logger;
        _sqsClient = sqsClient;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workerId = Guid.NewGuid();

        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = await _sqsClient.ReceiveMessagesAsync();

            foreach (var message in messages)
            {
                _logger.LogInformation("Worker {workerId} received message {messageContent} at {time}",
                    workerId,
                    message.Body,
                    DateTimeOffset.Now);

                await _sqsClient.RemoveMessageAsync(message);
            }

            await Task.Delay(int.Parse(_configuration["DelayTime"]), stoppingToken);
        }
    }
}
