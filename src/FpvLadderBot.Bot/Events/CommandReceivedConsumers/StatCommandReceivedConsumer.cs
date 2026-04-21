namespace FpvLadderBot.Events.CommandReceivedConsumers;

public class StatCommandReceivedConsumer(ITelegramBotClient botClient, IScopedMediator mediator)
    : CommandReceivedConsumerBase(Command.Stat, botClient, mediator) {
    private readonly IScopedMediator _mediator = mediator;
    
    protected override async Task ConsumeAndGetReply(long userId, long chatId, int? replyToMessageId, string[] args,
        bool isBotAdmin,
        CancellationToken cancellationToken) {
        Response<GetStatisticsResult> statistics = await _mediator
            .CreateRequestClient<GetStatistics>()
            .GetResponse<GetStatisticsResult>(new GetStatistics(), cancellationToken);

        string text =
            $"Пилотов: {statistics.Message.PilotCount}".ToEscapedMarkdownV2() + "\n" +
            $"Подписчиков: {statistics.Message.SubscriberCount}".ToEscapedMarkdownV2() + "\n" +
            $"Подписок: {statistics.Message.SubscribersCount}".ToEscapedMarkdownV2();

        await _botClient.SendMessage(
            chatId,
            text,
            ParseMode.MarkdownV2,
            cancellationToken: cancellationToken
        );
    }
}