namespace FpvLadderBot.Events.CallbackReceivedConsumers;

public class SubscriptionControlActionCallbackReceivedConsumer(IScopedMediator mediator) : IMediatorConsumer<CallbackReceived> {
    public async Task Consume(ConsumeContext<CallbackReceived> context) {
        if (context.Message is not {
                Data: NavigationData.ActionData {
                    Action: var action and (Actions.Subscribe or Actions.Unsubscribe),
                    Data: { } pilotId,
                    NewThread: var newThread
                },
                ChatId: var chatId,
                MessageId: var messageId,
                ChatType: var chatType,
                UserId: var userId
            }) {
            return;
        }

        CancellationToken cancellationToken = context.CancellationToken;

        switch (action) {
            case Actions.Subscribe:
                await mediator.Send(new AddSubscription(chatId, pilotId), cancellationToken);
                break;
            case Actions.Unsubscribe:
                await mediator.Send(new RemoveSubscription(chatId, pilotId), cancellationToken);
                break;
            default:
                throw new UnreachableException();
        }

        await mediator.Send(new CallbackReceived(
            new NavigationData.CommandData(Command.Find, pilotId),
            newThread ? null : messageId,
            chatId,
            chatType,
            userId,
            context.Message.IsBotAdmin
        ), cancellationToken);

        // var popResponse = await _mediator
        //     .CreateRequestClient<PopBackNavigation>()
        //     .GetResponse<BackNavigation, EmptyNavigation>(new PopBackNavigation(UserId: userId, ChatId: chatId), cancellationToken);
        //
        // if (popResponse.Is<BackNavigation>(out var popResult) && popResult.Message is { } pop) {
        // }
    }
}
