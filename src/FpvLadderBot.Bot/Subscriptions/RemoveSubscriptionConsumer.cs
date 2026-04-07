namespace FpvLadderBot.Subscriptions;

public class RemoveSubscriptionConsumer(AppDbContext db) : IMediatorConsumer<RemoveSubscription> {
    public async Task Consume(ConsumeContext<RemoveSubscription> context) {
        if (context.Message is not {
                PilotId: var pilotId,
                ChatId: var chatId
            }) {
            return;
        }

        CancellationToken cancellationToken = context.CancellationToken;

        SubscriptionEntity? entity = await db.Subscriptions.FindAsync([
            chatId, pilotId
        ], cancellationToken);
        if (entity is null) {
            return;
        }

        db.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }
}
