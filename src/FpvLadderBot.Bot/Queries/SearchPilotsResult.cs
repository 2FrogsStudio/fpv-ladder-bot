namespace FpvLadderBot.Queries;

public record SearchPilotsResult((string Name, string PilotId)[] Pilots);
