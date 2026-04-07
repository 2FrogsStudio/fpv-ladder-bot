CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

IHost host = Host.CreateApplicationBuilder(args)
    .AddLogging()
    .AddMemoryCache()
    .AddDatabase()
    .AddQuartz()
    .AddMassTransit()
    .AddTelegramBot()
    .AddFpvLadderClient()
    .Build();

host
    .MigrateDatabase()
    .Run();
