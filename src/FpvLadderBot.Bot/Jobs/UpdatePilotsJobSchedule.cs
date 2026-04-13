using MassTransit.Scheduling;

namespace FpvLadderBot.Jobs;

public class UpdatePilotsJobSchedule : DefaultRecurringSchedule {
    public UpdatePilotsJobSchedule(bool isDevelopment) {
        TimeZoneId = TimeZoneInfo.Utc.Id;
        // todo: pass through configuration
        CronExpression = "0 0 8-20/1 1/1 * ? *"; // every 4th hour from 8 through 20
        MisfirePolicy = isDevelopment ? MissedEventPolicy.Skip : MissedEventPolicy.Default;
    }
}
