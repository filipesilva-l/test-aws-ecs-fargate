using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Worker;

public class SqsClient
{
    private readonly IAmazonSQS _client;
    private readonly string _queueName;
    private readonly ILogger<SqsClient> _logger;
    private string? _queueUrl;

    public SqsClient(IConfiguration configuration, ILogger<SqsClient> logger)
    {
        var credentials = new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]);
        _client = new AmazonSQSClient(credentials, region: RegionEndpoint.SAEast1);

        _queueName = configuration["QueueName"];

        logger.LogInformation("AWS access: {access}", configuration["AWS:AccessKey"]);
        logger.LogInformation("AWS queue: {queue}", _queueName);
        _logger = logger;
    }

    private async Task<string> GetQueueUrlAsync()
    {
        _queueUrl = _queueUrl ?? (await _client.GetQueueUrlAsync(_queueName)).QueueUrl;

        return _queueUrl;
    }

    public async Task<List<Message>> ReceiveMessagesAsync()
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = await GetQueueUrlAsync(),
            MaxNumberOfMessages = 10
        };

        var response = await _client.ReceiveMessageAsync(request);

        return response.Messages;
    }

    public async Task RemoveMessageAsync(Message message)
    {
        var request = new DeleteMessageRequest(await GetQueueUrlAsync(), message.ReceiptHandle);

        var response = await _client.DeleteMessageAsync(request);

        _logger.LogInformation("Deleted message {messageBody} with response {responseCode}", message.Body, (int)response.HttpStatusCode);
    }
}
