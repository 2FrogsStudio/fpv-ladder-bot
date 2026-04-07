namespace FpvLadderBot.Queries;

public record GetPilotResult(
    string PilotId,
    string Fio,
    float Rating,
    uint Position,
    int Subscribers,
    DateTimeOffset Updated
);
