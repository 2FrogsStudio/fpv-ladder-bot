namespace FpvLadderBot.Subscriptions;

public class FindSubscriptionConsumer(AppDbContext db) : IMediatorConsumer<FindSubscription> {
    public async Task Consume(ConsumeContext<FindSubscription> context) {
        CancellationToken cancellationToken = context.CancellationToken;

        SubscriptionEntity? subscription =
            await db.Subscriptions.FindAsync([context.Message.ChatId, context.Message.PilotId],
                cancellationToken);

        if (subscription is not null) {
            await context.RespondAsync(new SubscriptionFound());
        } else {
            await context.RespondAsync(new SubscriptionNotFound());
        }
    }
}
