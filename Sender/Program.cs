using Microsoft.Extensions.Configuration;
using Sender;

IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .AddUserSecrets<SqsClient>()
        .Build();

var client = new SqsClient(config);

var nouns = new[] { "top", "coat", "turn", "grain", "person", "journey", "snail", "trousers", "lettuce", "appliance", "battle", "zipper", "pollution", "quicksand", "beggar", "elbow", "kite", "governor", "toad", "bun", "sense", "beetle", "ghost", "crate", "lettuce", "line", "cord", "hate", "arm", "basketball" };
var adjectives = new[] { "silent", "wretched", "black", "threatening", "lamentable", "wide-eyed", "beneficial", "ready", "curvy", "roomy", "crazy", "receptive", "naughty", "hulking", "humorous", "overconfident", "deeply", "crowded", "beneficial", "agreeable", "gruesome", "next", "abortive", "special", "nonstop", "aberrant", "unwieldy", "young", "neat", "lovely" };

for (var i = 0; i <= 20; i++)
{
    var messages = nouns.Select(n =>
    {
        var adjective = adjectives[new Random().Next(0, 30)];

        var word = $"{n} {adjective}";

        Console.WriteLine(word);

        return word;
    });

    foreach (var messagesChunk in messages.Chunk(10))
        await client.SendBatchAsync(messagesChunk);

    Console.WriteLine($"Batch {i} sent");
}

Console.WriteLine("Finished");

