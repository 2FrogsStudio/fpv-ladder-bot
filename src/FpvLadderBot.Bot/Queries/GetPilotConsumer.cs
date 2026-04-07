namespace FpvLadderBot.Queries;

public class GetPilotConsumer : IMediatorConsumer<GetPilot> {
    private readonly AppDbContext _db;

    public GetPilotConsumer(AppDbContext db) {
        _db = db;
    }

    public async Task Consume(ConsumeContext<GetPilot> context) {
        CancellationToken cancellationToken = context.CancellationToken;
        string pilotId = context.Message.PilotId;

        PilotEntity? entity = await _db.Pilots.FindAsync([pilotId], cancellationToken);

        if (entity is null) {
            await context.RespondAsync(new GetPilotNotFoundResult());
            return;
        }

        int subscribers =
            await _db.Subscriptions.CountAsync(s => s.PilotId == pilotId, cancellationToken);

        await context.RespondAsync(new GetPilotResult(
            entity.PilotId,
            entity.PilotName,
            entity.RatingValue,
            entity.Position,
            subscribers,
            TimeZoneInfo.ConvertTime(entity.Updated, Constants.RussianTimeZone)
        ));
    }
}
