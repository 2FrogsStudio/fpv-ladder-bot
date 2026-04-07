namespace FpvLadderBot.BackNavigations;

public record PushBackNavigation(long UserId, long ChatId, Guid Guid, string Name, NavigationData Data);
