using FpvLadderBot.Jobs;

namespace FpvLadderBot.Events.CommandReceivedConsumers;

public class StartUpdateJobConsumer(ITelegramBotClient botClient, IScopedMediator mediator, IBus bus)
    : CommandReceivedConsumerBase(Command.Update, botClient, mediator) {
    private readonly ITelegramBotClient _botClient = botClient;

    protected override async Task ConsumeAndGetReply(long userId, long chatId, int? replyToMessageId, string[] args,
        bool isBotAdmin,
        CancellationToken cancellationToken) {
        string formatter = DefaultEndpointNameFormatter.Instance.Consumer<UpdatePilotsJobConsumer>();
        var endpoint = new Uri($"queue:{formatter}");
        ISendEndpoint sendEndpoint = await bus.GetSendEndpoint(endpoint);
        Message message = await _botClient.SendMessage(chatId, "⏳Обновление запущено..".ToEscapedMarkdownV2(),
            ParseMode.MarkdownV2,
            replyToMessageId.HasValue
                ? new ReplyParameters { MessageId = replyToMessageId.Value }
                : null,
            cancellationToken: cancellationToken);
        await sendEndpoint.Send<UpdatePilotsJob>(new {
            __Header_ResponseChatId = chatId,
            __Header_UpdateMessageId = message.MessageId
        }, cancellationToken);
    }
}
