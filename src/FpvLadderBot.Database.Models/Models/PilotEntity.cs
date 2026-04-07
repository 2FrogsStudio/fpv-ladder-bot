using System.ComponentModel.DataAnnotations;

namespace FpvLadderBot.Models;

[PrimaryKey(nameof(PilotId))]
public class PilotEntity : IDatedEntity {
    [MaxLength(20)]
    public string PilotId { get; set; } = string.Empty;
    [MaxLength(100)]
    public string PilotName { get; set; } = string.Empty;
    public uint RatingValue { get; set; }
    public uint Position { get; set; }
    public DateTimeOffset LastEventDate { get; set; }

    public ICollection<SubscriptionEntity> Subscriptions { get; set; } = new List<SubscriptionEntity>();

    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Updated { get; set; }
}
