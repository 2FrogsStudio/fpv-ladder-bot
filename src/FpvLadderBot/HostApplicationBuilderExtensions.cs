using System.Text.Json;
using FpvLadderBot.Events.PilotRatingChangedConsumers;
using FpvLadderBot.Serilog;
using FpvLadderBot.Subscriptions;
using Quartz;
using Serilog.Enrichers.Sensitive;
using Telegram.Bot;

namespace FpvLadderBot;

public static class HostApplicationBuilderExtensions {
    public static HostApplicationBuilder AddLogging(this HostApplicationBuilder builder) {
        builder.Services.AddLogging(loggingBuilder => {
            loggingBuilder.AddConfiguration(builder.Configuration);
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.WithSensitiveDataMasking(options => {
                    options.MaskingOperators.Clear();
                    options.MaskProperties.Clear();
                    options.MaskingOperators.Add(
                        new RegexWithSecretMaskingOperator(@"https:\/\/api.telegram.org\/bot(?'secret'.*?)\/")
                    );
                })
                // .Destructure.With<IncludePublicNotNullFieldsPolicy>()
                .Destructure.With<SerializeJsonElementPolicy>()
                .CreateLogger());
            loggingBuilder.AddSentry(options => {
                if (builder.Environment.IsDevelopment()) {
                    options.Dsn = "";
                    return;
                }
                options.Environment = builder.Environment.EnvironmentName;
            });
        });
        return builder;
    }

    public static HostApplicationBuilder AddMemoryCache(this HostApplicationBuilder builder) {
        builder.Services.AddMemoryCache()
            .Configure<MemoryCacheOptions>(builder.Configuration.GetSection("MemoryCache"));
        return builder;
    }

    public static HostApplicationBuilder AddMassTransit(this HostApplicationBuilder builder) {
        builder.Services
            .AddMassTransit(x => {
                x.AddQuartzConsumers();
                x.AddConsumers(type => !type.IsAssignableToGenericType(typeof(IMediatorConsumer<>)),
                    typeof(AddSubscriptionConsumer).Assembly);
                if (builder.Configuration["AMQP_URI"] is { } amqpUri) {
                    x.UsingRabbitMq((context, cfg) => {
                        cfg.Host(amqpUri);
                        cfg.ConfigureJsonSerializerOptions(AddJsonBotApiJsonSerializerOptions);
                        cfg.ConfigureEndpoints(context);
                    });
                } else {
                    x.UsingInMemory((context, cfg) => {
                        cfg.ConfigureJsonSerializerOptions(AddJsonBotApiJsonSerializerOptions);
                        cfg.ConfigureEndpoints(context);
                    });
                }
            })
            .AddMediator(x => {
                x.AddConsumers(type => type.IsAssignableToGenericType(typeof(IMediatorConsumer<>)),
                    typeof(NotifySubscribersPilotRatingChangedConsumer).Assembly);
            });
        return builder;
    }

    public static HostApplicationBuilder AddQuartz(this HostApplicationBuilder builder) {
        builder.Services.AddQuartz(q => {
            q.MisfireThreshold = TimeSpan.FromMinutes(10);
            q.SetProperty("quartz.scheduler.idleWaitTime",
                TimeSpan.FromMinutes(10).TotalMicroseconds.ToString(CultureInfo.InvariantCulture));

            q.UsePersistentStore(s => {
                string provider = builder.Configuration.GetValue("Provider", "Postgres");
                string connectionString = builder.Configuration.GetConnectionString(provider) ??
                                          throw new InvalidOperationException();
                switch (provider) {
                    case "Sqlite":
                        s.UseMicrosoftSQLite(server => server.ConnectionString = connectionString);
                        break;
                    case "Postgres":
                        s.UsePostgres(server => server.ConnectionString = connectionString);
                        // disabled die to avoid using active connection of free tier of neon.tech
                        // s.UseClustering();
                        break;
                    default:
                        throw new Exception($"Unsupported provider: {provider}");
                }

                s.UseSystemTextJsonSerializer();
            });
        });
        return builder;
    }

    private static JsonSerializerOptions AddJsonBotApiJsonSerializerOptions(JsonSerializerOptions options) {
        JsonBotAPI.Configure(options);
        return options;
    }
}
