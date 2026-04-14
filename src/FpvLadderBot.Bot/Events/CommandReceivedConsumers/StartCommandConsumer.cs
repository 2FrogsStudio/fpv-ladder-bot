namespace FpvLadderBot.Events.CommandReceivedConsumers;

public class StartCommandConsumer(ITelegramBotClient botClient, IScopedMediator mediator) : CommandReceivedConsumerBase(
    Command.Start, botClient,
    mediator) {
    private readonly IScopedMediator _mediator = mediator;

    protected override async Task ConsumeAndGetReply(long userId, long chatId, int? replyToMessageId, string[] args,
        bool isBotAdmin,
        CancellationToken cancellationToken) {
        IEnumerable<InlineKeyboardButton[]> commandMenuRows = CommandHelpers.CommandAttributeByCommand
            .Where(pair => pair.Value?.InlineName is not null)
            .Select(pair => {
                string name = pair.Value?.InlineName ?? throw new NullReferenceException();
                var data = new NavigationData.PipelineData(pair.Value.Pipeline);
                return new InlineKeyboardButton(name) {
                    CallbackData = JsonSerializer.Serialize(data)
                };
            })
            .Split(3);

        GetSubscriptionsByChatIdResult subscriptions = (await _mediator
                .CreateRequestClient<GetSubscriptionsByChatId>()
                .GetResponse<GetSubscriptionsByChatIdResult>(new GetSubscriptionsByChatId(chatId), cancellationToken))
            .Message;

        InlineKeyboardButton[][] pilotButtonRows = subscriptions.Subscriptions
            .Select(s => new InlineKeyboardButton($"{s.Fio} ({s.Rating})") {
                CallbackData = JsonSerializer.Serialize(new NavigationData.CommandData(Command.Find, s.PilotId))
            }).Split(1).ToArray();

        Text = pilotButtonRows.Length>0 ? "Мои подписки" : "Пока нет подписок";
        InlineKeyboard = pilotButtonRows.Union(commandMenuRows);
        await _mediator.Send(new ResetBackNavigation(userId, chatId), cancellationToken);
    }
}
