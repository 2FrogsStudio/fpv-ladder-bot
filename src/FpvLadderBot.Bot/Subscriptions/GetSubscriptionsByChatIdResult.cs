namespace FpvLadderBot.Subscriptions;

public record GetSubscriptionsByChatIdResult((string Fio, uint Rating, string PilotId)[] Subscriptions);
