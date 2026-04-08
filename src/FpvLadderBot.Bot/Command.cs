using FpvLadderBot.Attributes;

namespace FpvLadderBot;

public enum Command {
    Unknown,

    [Command(
        Text = "/start",
        Description = "Главное меню"
    )]
    Start,

    [Command(
        Text = "/find",
        InlineName = "🔎 Поиск",
        Pipeline = Pipeline.Find,
        Description = "Найти пилота по имени"
    )]
    [CommandArg("[Запрос]",
        "Имя для поиска")]
    Find,

    [Command(Text = "/update",
        Description = "Обновление",
        IsInitCommand = false,
        IsAdminCommand = true)]
    Update,

    [Command(Text = "/reschedule_job",
        Description = "Обновление джобы",
        IsInitCommand = false,
        IsAdminCommand = true)]
    RescheduleJob,

    [Command(
        Text = "/help",
        Description = "Список доступных команд"
    )]
    Help
}
