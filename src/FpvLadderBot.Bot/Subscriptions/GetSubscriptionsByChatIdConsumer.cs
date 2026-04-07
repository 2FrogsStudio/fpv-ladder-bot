namespace FpvLadderBot.Subscriptions;

public class GetSubscriptionsByChatIdConsumer(AppDbContext db) : IMediatorConsumer<GetSubscriptionsByChatId> {
    public async Task Consume(ConsumeContext<GetSubscriptionsByChatId> context) {
        CancellationToken cancellationToken = context.CancellationToken;

        var subscriptionEntities = await db.Subscriptions
            .Where(s => s.ChatId == context.Message.ChatId)
            .OrderBy(s => s.Pilot.PilotName)
            .Select(s => new { s.Pilot.PilotName, s.Pilot.RatingValue, s.PilotId })
            .ToArrayAsync(cancellationToken);

        await context.RespondAsync(new GetSubscriptionsByChatIdResult(
            subscriptionEntities
                .Select(s => (s.PilotName, s.RatingValue, s.PilotId))
                .ToArray()
        ));
    }
}
