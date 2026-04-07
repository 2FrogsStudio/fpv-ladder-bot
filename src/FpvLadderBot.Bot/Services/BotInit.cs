namespace FpvLadderBot.Services;

internal class BotInit(ITelegramBotClient botClient, ILogger<BotInit> logger, IServiceProvider serviceProvider)
    : IHostedService {
    public async Task StartAsync(CancellationToken cancellationToken) {
        logger.LogInformation("Initialize bot (commands, etc)");
        await InitCommands(cancellationToken);
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IScopedMediator>();
        await mediator.Send(new InitUpdaterJob(false), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    private async Task InitCommands(CancellationToken cancellationToken) {
        IEnumerable<BotCommand> commands = CommandHelpers.CommandAttributeByCommand.Values
            .Where(d => d is { IsInitCommand: true })
            .Select(d => new BotCommand {
                Command = d?.Text ?? throw new ArgumentNullException(nameof(d)),
                Description = d.Description ?? string.Empty
            });
        await botClient.SetMyCommands(commands, cancellationToken: cancellationToken);
    }
}
