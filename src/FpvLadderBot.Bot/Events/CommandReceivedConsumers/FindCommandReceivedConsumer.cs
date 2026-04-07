using System.Text;

namespace FpvLadderBot.Events.CommandReceivedConsumers;

public class FindCommandReceivedConsumer(ITelegramBotClient botClient, IScopedMediator mediator)
    : CommandReceivedConsumerBase(Command.Find,
        botClient, mediator) {
    private readonly IScopedMediator _mediator = mediator;

    protected override async Task ConsumeAndGetReply(long userId, long chatId, int? replyToMessageId, string[] args,
        bool isBotAdmin,
        CancellationToken cancellationToken) {
        while (true) {
            switch (args) {
                case [{ } id, ..] when id.Split('_').Length == 3:
                    await ComposePilotInfo(chatId, id ?? throw new InvalidOperationException(), cancellationToken);
                    return;
                case [{ } search, ..]:
                    (string Name, string PilotId)[] pilots = await SearchPilots(search, cancellationToken);
                    switch (pilots) {
                        case []:
                            Text = "Участники не найдены".ToEscapedMarkdownV2();
                            return;
                        case [var pilot]:
                            args = [pilot.PilotId];
                            continue;
                        default:
                            //todo: pagination
                            InlineKeyboardButton[] pilotButtons = pilots
                                .Select(p => {
                                    string data = JsonSerializer.Serialize(
                                        new NavigationData.CommandData(Command.Find, p.PilotId));
                                    return new InlineKeyboardButton(p.Name) {
                                        CallbackData = data
                                    };
                                })
                                .ToArray();
                            Text = "Выберите участника";
                            InlineKeyboard = pilotButtons.Split(1);
                            return;
                    }
                default:
                    Text = CommandHelpers.HelpByCommand[Command.Find];
                    return;
            }
        }
    }

    private async Task ComposePilotInfo(long chatId, string pilotId,
        CancellationToken cancellationToken) {
        await _mediator.Send(new UpdatePilot(pilotId), cancellationToken);

        Response<GetPilotResult, GetPilotNotFoundResult> result = await _mediator
            .CreateRequestClient<GetPilot>()
            .GetResponse<GetPilotResult, GetPilotNotFoundResult>(new GetPilot(pilotId),
                cancellationToken);
        if (result.Is<GetPilotResult>(out Response<GetPilotResult>? pilotResponse) &&
            pilotResponse.Message is { } pilot) {
            var buttons = new List<InlineKeyboardButton>();

            Response<SubscriptionFound, SubscriptionNotFound> findSubscriptionResponse = await _mediator
                .CreateRequestClient<FindSubscription>()
                .GetResponse<SubscriptionFound, SubscriptionNotFound>(new FindSubscription(chatId, pilot.PilotId),
                    cancellationToken);
            if (findSubscriptionResponse.Is<SubscriptionFound>(out _)) {
                buttons.Add(new InlineKeyboardButton("Отписаться") {
                    CallbackData =
                        JsonSerializer.Serialize(
                            new NavigationData.ActionData(Actions.Unsubscribe, pilot.PilotId))
                });
            }

            if (findSubscriptionResponse.Is<SubscriptionNotFound>(out _)) {
                buttons.Add(new InlineKeyboardButton("Подписаться") {
                    CallbackData =
                        JsonSerializer.Serialize(new NavigationData.ActionData(Actions.Subscribe, pilot.PilotId))
                });
            }

            string link = new StringBuilder(Constants.FpvLadderUrl)
                .Append("pilot/")
                .Append(pilot.PilotId.Split(':')[0].Replace('_','/'))
                .Append(".html")
                .ToString();
            
            Text =
                $"{pilot.Fio}".ToEscapedMarkdownV2() + "\n" +
                $"Рейтинг: {pilot.Rating}".ToEscapedMarkdownV2() + "\n" +
                $"Позиция: {pilot.Position}".ToEscapedMarkdownV2() + "\n" +
                $"Подписчиков в боте: {pilot.Subscribers}".ToEscapedMarkdownV2() + "\n" +
                $"Обновлено: {pilot.Updated:dd.MM.yyyy H:mm} (МСК)".ToEscapedMarkdownV2() + "\n" +
                link.ToEscapedMarkdownV2();
            InlineKeyboard = buttons.Split(1);
            return;
        }

        if (result.Is<GetPilotNotFoundResult>(out _)) {
            Text = $"Пилот с идентификатором {pilotId} не найден".ToEscapedMarkdownV2();
            return;
        }

        throw new UnreachableException();
    }

    private async Task<(string Name, string PilotId)[]> SearchPilots(string search,
        CancellationToken cancellationToken) {
        Response<SearchPilotsResult> response = await _mediator
            .CreateRequestClient<SearchPilots>()
            .GetResponse<SearchPilotsResult>(new SearchPilots(search), cancellationToken);
        return response.Message.Pilots;
    }

    private static bool TryGetPilotId(string url, out string? pilotId) {
        var baseUri = new Uri(Constants.FpvLadderUrl);
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
            || string.IsNullOrWhiteSpace(uri.Host)) {
            uri = new Uri(baseUri, url);
        }

        if (uri.Host == baseUri.Host
            && uri is { AbsolutePath: "/pilots/" }) {
            pilotId = HttpUtility.ParseQueryString(uri.Query).Get("id");
            if (!string.IsNullOrWhiteSpace(pilotId)) {
                pilotId = uri.PathAndQuery;
                return true;
            }
        }

        pilotId = null;
        pilotId = null;
        return false;
    }
}
