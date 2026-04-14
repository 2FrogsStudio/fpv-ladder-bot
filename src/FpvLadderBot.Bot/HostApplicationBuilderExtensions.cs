using System.Net;
using FpvLadderBot.Services;

namespace FpvLadderBot;

public static class HostApplicationBuilderExtensions {
    public static HostApplicationBuilder AddTelegramBot(this HostApplicationBuilder builder) {
        string? botToken = builder.Configuration["Bot:Token"];
        if (botToken is null or "BOT_API_TOKEN_HERE") {
            throw new NullReferenceException(
                "Provide token of your Telegram bot with `Bot__Token` environment variable");
        }

        builder.Services.AddMemoryCache();
        builder.Services.AddHttpClient("TelegramBotClient")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler {
                Proxy = builder.Environment.IsDevelopment() ? new WebProxy("socks5://localhost:7070") : null,
                UseProxy = builder.Environment.IsDevelopment()
            })
            .AddTypedClient<ITelegramBotClient>(client => new TelegramBotClient(botToken, client));
        builder.Services.AddHostedService<BotInit>()
            .AddHostedService<PullingService>()
            .AddSingleton<IUpdateHandler, UpdateHandler>();
        return builder;
    }
}
