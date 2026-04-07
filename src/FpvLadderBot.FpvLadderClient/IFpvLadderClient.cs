namespace FpvLadderBot;

public interface IFpvLadderClient {
    Task<PilotRating?> GetPilotInfo(string pilotId, CancellationToken cancellationToken);
    Task<Pilot[]> FindPilots(string searchQuery, CancellationToken cancellationToken);
}
