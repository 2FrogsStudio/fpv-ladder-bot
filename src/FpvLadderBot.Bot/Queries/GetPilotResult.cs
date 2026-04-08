namespace FpvLadderBot.Queries;

public record GetPilotResult(
    string PilotId,
    string Name,
    float Rating,
    uint Position,
    int Subscribers,
    DateTimeOffset Updated
);
