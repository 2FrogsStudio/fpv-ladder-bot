using Microsoft.Extensions.Configuration;
using Quartz.Util;
using Telegram.Bot.Exceptions;

namespace FpvLadderBot.Services;

internal class UpdateHandler(
    ILogger<UpdateHandler> logger,
    IServiceProvider serviceProvider,
    IConfiguration configuration)
    : IUpdateHandler {
    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update,
        CancellationToken cancellationToken) {
        using IDisposable? updateIdScope = logger.BeginScope(new Dictionary<string, object> {
            { "UpdateId", update.Id.ToString() },
            { "UpdateType", update.Type.ToString() }
        });

        logger.LogDebug("Update received: {@Update}", update);

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        try {
            await publishEndpoint.Publish(new UpdateReceived(update, IsBotAdmin(update)), cancellationToken);
        } catch (Exception ex) {
            logger.LogError(ex, "UpdateReceived failed: {@Update}", update);
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken) {
        logger.LogError(exception, "Telegram API Error");
        if (exception is ApiRequestException) {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }

    private bool IsBotAdmin(Update update) {
        long? userId = update.Message?.From?.Id ?? update.CallbackQuery?.From.Id;

        if (userId is null) {
            return false;
        }

        string? config = configuration.GetValue<string>("Bot:AdminIds");
        if (config is null || config.IsNullOrWhiteSpace()) {
            return false;
        }

        return config.Split(',', ';')
            .Select(long.Parse)
            .Contains(userId.Value);
    }
}
