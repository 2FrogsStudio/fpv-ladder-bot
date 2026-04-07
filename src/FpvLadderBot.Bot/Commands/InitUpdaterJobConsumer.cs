using FpvLadderBot.Jobs;
using Quartz;

namespace FpvLadderBot.Commands;

public class InitUpdaterJobConsumer(IHostEnvironment hostEnvironment, IBus bus, ISchedulerFactory schedulerFactory)
    : IMediatorConsumer<InitUpdaterJob> {
    public async Task Consume(ConsumeContext<InitUpdaterJob> context) {
        CancellationToken cancellationToken = context.CancellationToken;
        var schedule = new UpdatePilotsJobSchedule(hostEnvironment.IsDevelopment());
        string? currentCron =
            await GetCurrentCronSchedule(schedule.ScheduleId, schedule.ScheduleGroup, cancellationToken);

        if (context.Message.CancelPrevious) {
            await CancelUpdaterJob(schedule.ScheduleId, schedule.ScheduleGroup);
        }

        if (context.Message.CancelPrevious || currentCron != schedule.CronExpression) {
            await ScheduleUpdaterJob(schedule, context.CancellationToken);
        }
    }

    private async Task CancelUpdaterJob(string scheduleId, string scheduleGroup) {
        ISendEndpoint sendEndpoint = await bus.GetSendEndpoint(new Uri("queue:quartz"));
        await sendEndpoint.CancelScheduledRecurringSend(scheduleId, scheduleGroup);
    }

    private async Task ScheduleUpdaterJob(UpdatePilotsJobSchedule schedule, CancellationToken cancellationToken) {
        ISendEndpoint sendEndpoint = await bus.GetSendEndpoint(new Uri("queue:quartz"));
        string formatter = DefaultEndpointNameFormatter.Instance.Consumer<UpdatePilotsJobConsumer>();
        var endpoint = new Uri($"queue:{formatter}");
        await sendEndpoint.ScheduleRecurringSend<UpdatePilotsJob>(endpoint, schedule, new { }, cancellationToken);
    }

    private async Task<string?> GetCurrentCronSchedule(string scheduleId,
        string scheduleGroup,
        CancellationToken cancellationToken) {
        IScheduler scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var triggerKey = new TriggerKey("Recurring.Trigger." + scheduleId,
            scheduleGroup);
        ITrigger? trigger = await scheduler.GetTrigger(triggerKey, cancellationToken);

        return trigger is ICronTrigger {
            CronExpressionString: { } cronExp
        }
            ? cronExp
            : null;
    }
}
