using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;

namespace Sender;

public class SqsClient
{
    private readonly AmazonSQSClient _client;
    private readonly string _queueName;
    private string? _queueUrl;

    public SqsClient(IConfiguration configuration)
    {
        var credentials = new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]);
        _client = new AmazonSQSClient(credentials, region: RegionEndpoint.SAEast1);

        Console.WriteLine(configuration["AWS:AccessKey"]);
        Console.WriteLine(configuration["QueueName"]);

        _queueName = configuration["QueueName"];
    }

    private async Task<string> GetQueueUrlAsync()
    {
        _queueUrl = _queueUrl ?? (await _client.GetQueueUrlAsync(_queueName)).QueueUrl;

        return _queueUrl;
    }

    public async Task SendBatchAsync(IEnumerable<string> messages)
    {
        var entries = messages.Select(m => new SendMessageBatchRequestEntry(Guid.NewGuid().ToString(), m)).ToList();

        var request = new SendMessageBatchRequest(await GetQueueUrlAsync(), entries);

        await _client.SendMessageBatchAsync(request);
    }
}
