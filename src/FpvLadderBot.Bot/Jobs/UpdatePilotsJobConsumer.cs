namespace FpvLadderBot.Jobs;

public class UpdatePilotsJobConsumer(AppDbContext db, IScopedMediator mediator, ITelegramBotClient bot) : IConsumer<UpdatePilotsJob> {
    public async Task Consume(ConsumeContext<UpdatePilotsJob> context) {
        CancellationToken cancellationToken = context.CancellationToken;
        string[] pilotIds = await db.Pilots
            // update all pilots to get actual info in find dialog too
            // .Where(p => p.Subscriptions.Count > 0)
            .Select(p => p.PilotId)
            .ToArrayAsync(cancellationToken);

        var exceptions = new List<Exception>();
        uint? chatId = context.Headers.Get<uint>("ResponseChatId");
        int? messageId = context.Headers.Get<int>("UpdateMessageId");

        for (int index = 0; index < pilotIds.Length; index++) {
            string pilotId = pilotIds[index];
            try {
                await mediator.Send(new UpdatePilot(pilotId, true), cancellationToken);
                await UpdateProgressMessage(chatId, messageId, index + 1, pilotIds.Length, cancellationToken);
            } catch (Exception ex) {
                exceptions.Add(ex);
            }
        }

        try {
            await FinishedProgressNotification(chatId, messageId, exceptions.Count > 0, cancellationToken);
        } catch (Exception ex) {
            exceptions.Add(ex);
        }

        if (exceptions.Count > 0) {
            throw new AggregateException("Encountered errors while trying to update pilots.", exceptions);
        }
    }

    private async Task UpdateProgressMessage(uint? chatId, int? messageId, int progress, int count,
        CancellationToken cancellationToken) {
        if (chatId is null || messageId is null) {
            return;
        }

        double totalProgress = Math.Round((float)progress / count * 100, 0);
        await bot.EditMessageText(chatId.Value, messageId.Value,
            $"⏳Обновление {totalProgress}%..".ToEscapedMarkdownV2(), ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }

    private async Task FinishedProgressNotification(uint? chatId, int? messageId, bool failed,
        CancellationToken cancellationToken) {
        if (chatId is null || messageId is null) {
            return;
        }

        string text = failed
            ? "🚨 Ошибка при обновлении рейтинга, проверьте логи"
            : "✅ Обновление завершено";
        await bot.EditMessageText(chatId.Value, messageId.Value, text,
            cancellationToken: cancellationToken);
    }
}
