namespace FpvLadderBot.Queries;

public class GetStatisticsConsumer(AppDbContext db) : IMediatorConsumer<GetStatistics> {
    public async Task Consume(ConsumeContext<GetStatistics> context) {
        uint pilotCount = (uint)db.Pilots.GroupBy(p => p.PilotId).Count();
        uint subscriberCount = (uint)db.Subscriptions.GroupBy(p => p.ChatId).Count();
        uint subscriptionCount = (uint)db.Subscriptions.Count();

        await context.RespondAsync(new GetStatisticsResult(
            pilotCount,
            subscriberCount,
            subscriptionCount
        ));
    }
}
