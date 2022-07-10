using Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<SqsClient>();

        services.AddHostedService<WorkerTestFargate>();
    })
    .Build();

await host.RunAsync();
