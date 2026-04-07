namespace FpvLadderBot.Events;

public record PilotRatingChanged(
    string PilotId,
    float OldRating,
    uint OldPosition
);
