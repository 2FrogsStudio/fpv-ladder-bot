using Microsoft.Extensions.Caching.Memory;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace FpvLadderBot;

public class FpvLadderClient(HttpClient httpClient, IMemoryCache memoryCache) : IFpvLadderClient {
    private async Task<PilotRating[]> GetPilotRatings(CancellationToken cancellationToken) {
        return await memoryCache.GetOrCreateAsync<PilotRating[]>("PilotsIndex", async _ => {
            Stream indexYamlStream = await httpClient.GetStreamAsync($"index.yaml?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}", cancellationToken);
            var parser = new Parser(new StreamReader(indexYamlStream));
            IDeserializer deserializer = new DeserializerBuilder().Build();
            parser.Consume<StreamStart>();
            var piliInfos = new List<PilotIndexModel>();
            while (parser.Accept<DocumentStart>(out DocumentStart? _)) {
                var pilotInfo = deserializer.Deserialize<PilotIndexModel>(parser);
                pilotInfo.Class = pilotInfo.Class.Replace("drone-racing > ", string.Empty);
                piliInfos.Add(pilotInfo);
            }

            string[] classes = piliInfos.DistinctBy(p => p.Class).Select(p => p.Class).ToArray();
            var list = new List<PilotRating>();
            foreach (string @class in classes) {
                list.AddRange(
                    piliInfos
                        .Where(p => p.Class == @class)
                        .OrderByDescending(p => p.RatingValue)
                        .ThenByDescending(p => p.EventDate)
                        .Select((pilot, i) => new PilotRating(
                            pilot.PilotId.Replace('/', '_') + ':' + pilot.Class,
                            string.Format($"{pilot.PilotName} [{pilot.Class}]"), pilot.RatingValue,
                            pilot.EventDate, (uint)i + 1)));
            }
            return list.ToArray();
        }, new MemoryCacheEntryOptions {
            SlidingExpiration = TimeSpan.FromTicks(5),
            Size = 1
        }) ?? throw new InvalidOperationException();
    }

    public async Task<PilotRating?> GetPilotInfo(string pilotId, CancellationToken cancellationToken) {
        PilotRating[] pilots = await GetPilotRatings(cancellationToken);
        return pilots.SingleOrDefault(pilot => pilot.PilotId.Equals(pilotId));
    }

    public async Task<Pilot[]> FindPilots(string searchQuery, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(searchQuery)) {
            return [];
        }

        PilotRating[] pilots = await GetPilotRatings(cancellationToken);

        return pilots
            .Where(p => p.PilotId.Equals(searchQuery) || p.PilotName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
            .ToArray<Pilot>();
    }
}
