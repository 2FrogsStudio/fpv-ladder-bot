namespace FpvLadderBot.Subscriptions;

public class AddSubscriptionConsumer(AppDbContext db, IScopedMediator mediator) : IMediatorConsumer<AddSubscription> {
    public async Task Consume(ConsumeContext<AddSubscription> context) {
        if (context.Message is not {
                PilotId: var pilotId,
                ChatId: var chatId
            }) {
            return;
        }

        CancellationToken cancellationToken = context.CancellationToken;
        Response<GetPilotResult, GetPilotNotFoundResult> response = await mediator
            .CreateRequestClient<GetPilot>()
            .GetResponse<GetPilotResult, GetPilotNotFoundResult>(new GetPilot(pilotId),
                cancellationToken);

        if (response.Is<GetPilotNotFoundResult>(out _)) {
            throw new InvalidOperationException("Pilot not found by Url") {
                Data = { { "PilotId", pilotId } }
            };
        }

        if (!response.Is<GetPilotResult>(out Response<GetPilotResult>? result) || result.Message is not { } pilot) {
            throw new UnreachableException();
        }

        SubscriptionEntity entity =
            await db.Subscriptions.FindAsync([chatId, pilotId], cancellationToken)
            ?? new SubscriptionEntity {
                ChatId = chatId,
                PilotId = pilot.PilotId,
                Pilot = await db.Pilots.FindAsync([pilotId], cancellationToken) ??
                        throw new InvalidOperationException()
            };

        if (db.Entry(entity).State is EntityState.Detached) {
            db.Add(entity);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
