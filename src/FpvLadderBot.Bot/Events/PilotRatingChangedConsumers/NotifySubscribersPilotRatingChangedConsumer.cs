using Telegram.Bot.Exceptions;

namespace FpvLadderBot.Events.PilotRatingChangedConsumers;

public class NotifySubscribersPilotRatingChangedConsumer(
    AppDbContext db,
    ITelegramBotClient bot,
    IScopedMediator mediator,
    ILogger<NotifySubscribersPilotRatingChangedConsumer> logger)
    : IConsumer<PilotRatingChanged> {
    public async Task Consume(ConsumeContext<PilotRatingChanged> context) {
        var subscriptions = await db.Subscriptions
            .Where(s => s.PilotId == context.Message.PilotId)
            .Select(s => new { chatId = s.ChatId, pilotId = s.PilotId })
            .ToArrayAsync(context.CancellationToken);

        var exceptions = new List<Exception>();

        foreach (var subscription in subscriptions) {
            try {
                await SendNotification(subscription.chatId, subscription.pilotId, context.Message,
                    context.CancellationToken);
            } catch (ApiRequestException ex) when (ex.ErrorCode is 403 &&
                                                   ex.Message is "Forbidden: bot was blocked by the user") {
                await RemoveBannedSubscription(subscription.chatId, context.CancellationToken);
            } catch (Exception ex) {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 0) {
            throw new AggregateException("Encountered errors while trying to update pilots.", exceptions);
        }
    }

    private async Task RemoveBannedSubscription(long chatId, CancellationToken cancellationToken) {
        SubscriptionEntity[] entities =
            await db.Subscriptions.Where(s => s.ChatId == chatId).ToArrayAsync(cancellationToken);
        db.Subscriptions.RemoveRange(entities);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task SendNotification(long chatId, string pilotId, PilotRatingChanged changed,
        CancellationToken cancellationToken) {
        Response<GetPilotResult> result = await mediator
            .CreateRequestClient<GetPilot>()
            .GetResponse<GetPilotResult>(new GetPilot(pilotId),
                cancellationToken);

        GetPilotResult pilot = result.Message;

        bool isIncreased = pilot.Rating >= changed.OldRating;

        string ratingDelta =
            isIncreased
                ? $"{changed.OldRating} + {pilot.Rating - changed.OldRating} → {pilot.Rating}"
                : $"{changed.OldRating} - {changed.OldRating - pilot.Rating} → {pilot.Rating}";

        string positionDelta =
            pilot.Position >= changed.OldPosition
                ? pilot.Position == changed.OldPosition
                    ? $"{pilot.Position}"
                    : $"{changed.OldPosition} + {pilot.Position - changed.OldPosition} → {pilot.Position}"
                : $"{changed.OldPosition} - {changed.OldPosition - pilot.Position} → {pilot.Position}";

        logger.LogInformation(
            "Pilot's rating updated: OldRating:{OldRating} NewRating:{NewRating} OldPosition:{OldPosition} NewPosition:{NewPosition}",
            changed.OldRating, pilot.Rating, changed.OldPosition, pilot.Position);

        string text =
            $"{(isIncreased ? "🚀" : "🔻")} Рейтинг обновлен ".ToEscapedMarkdownV2() + '\n' +
            $"{pilot.Fio}".ToEscapedMarkdownV2() + "\n" +
            $"Рейтинг: {ratingDelta}".ToEscapedMarkdownV2() + '\n' +
            $"Позиция: {positionDelta}".ToEscapedMarkdownV2() + '\n' +
            $"Подписчиков: {pilot.Subscribers}".ToEscapedMarkdownV2() + "\n" +
            $"Обновлено: {pilot.Updated:dd.MM.yyyy H:mm} (МСК)".ToEscapedMarkdownV2() + "\n" +
            PilotLinkFormatter.FormatPilotLink(pilot.PilotId);

        var buttons = new List<InlineKeyboardButton>();

        Response<SubscriptionFound, SubscriptionNotFound> findSubscriptionResponse = await mediator
            .CreateRequestClient<FindSubscription>()
            .GetResponse<SubscriptionFound, SubscriptionNotFound>(new FindSubscription(chatId, pilotId),
                cancellationToken);
        if (findSubscriptionResponse.Is<SubscriptionFound>(out _)) {
            buttons.Add(new InlineKeyboardButton("Отписаться") {
                CallbackData =
                    JsonSerializer.Serialize(
                        new NavigationData.ActionData(Actions.Unsubscribe, pilotId, true))
            });
        }

        if (findSubscriptionResponse.Is<SubscriptionNotFound>(out _)) {
            buttons.Add(new InlineKeyboardButton("Подписаться") {
                CallbackData =
                    JsonSerializer.Serialize(new NavigationData.ActionData(Actions.Subscribe, pilotId, true))
            });
        }

        buttons.Add(new InlineKeyboardButton("↩︎ Меню") {
            CallbackData = JsonSerializer.Serialize(new NavigationData.CommandData(Command.Start, newThread: true))
        });

        await bot.SendMessage(
            chatId,
            text,
            ParseMode.MarkdownV2,
            replyMarkup: new InlineKeyboardMarkup(buttons.Split(1)),
            cancellationToken: cancellationToken
        );
    }
}
