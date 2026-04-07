namespace FpvLadderBot.Queries;

public class SearchPilotsConsumer(IFpvLadderClient fpvLadderClient) : IMediatorConsumer<SearchPilots> {
    public async Task Consume(ConsumeContext<SearchPilots> context) {
        CancellationToken cancellationToken = context.CancellationToken;

        Pilot[] pilots = await fpvLadderClient.FindPilots(context.Message.SearchQuery, cancellationToken);

        await context.RespondAsync(new SearchPilotsResult(
            pilots
                .OrderBy(p => p.PilotName)
                .Select(p => (p.PilotName, p.PilotId))
                .ToArray()));
    }
}
