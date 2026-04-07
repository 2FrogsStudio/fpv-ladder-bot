namespace FpvLadderBot.Commands;

public class UpdatePilotConsumer(IFpvLadderClient fpvLadderClient, AppDbContext db, IBus bus) : IMediatorConsumer<UpdatePilot> {
    public async Task Consume(ConsumeContext<UpdatePilot> context) {
        CancellationToken cancellationToken = context.CancellationToken;
        string pilotId = context.Message.PilotId;
        bool forceUpdate = context.Message.ForceUpdate;

        PilotEntity? entity = await db.Pilots.FindAsync([pilotId], cancellationToken);
        if (entity is null || forceUpdate) {
            PilotRating? pilotInfo = await fpvLadderClient.GetPilotInfo(pilotId,cancellationToken);
            if (pilotInfo is null) {
                return;
            }

            float? oldRating = null;
            uint? oldPosition = null;
            bool isRatingChanged = false;

            if (entity is not null) {
                oldRating = entity.RatingValue;
                oldPosition = entity.Position;
                isRatingChanged = Math.Abs(pilotInfo.RatingValue - oldRating.Value) > 0;
            }

            entity ??= new PilotEntity();
            entity.PilotId = pilotId;
            entity.PilotName = pilotInfo.PilotName;
            entity.RatingValue = pilotInfo.RatingValue;
            entity.Position = pilotInfo.Position;
            entity.LastEventDate = pilotInfo.LastEventDate;

            if (db.Entry(entity).State is EntityState.Detached) {
                db.Add(entity);
            }

            await db.SaveChangesAsync(cancellationToken);

            if (isRatingChanged) {
                await bus.Publish(
                    new PilotRatingChanged(
                        pilotInfo.PilotId,
                        oldRating.GetValueOrDefault(),
                        oldPosition.GetValueOrDefault()
                    ), cancellationToken);
            }
        }
    }
}
