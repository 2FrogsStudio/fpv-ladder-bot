namespace FpvLadderBot.Models;

public record PilotRating(
    string PilotId,
    string PilotName,
    uint RatingValue,
    DateTimeOffset LastEventDate,
    uint Position) : Pilot(PilotId, PilotName);
