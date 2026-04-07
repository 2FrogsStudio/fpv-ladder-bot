using System.ComponentModel.DataAnnotations;

namespace FpvLadderBot.Models;

[PrimaryKey(nameof(ChatId), nameof(PilotId))]
public class SubscriptionEntity {
    public long ChatId { get; set; }

    [MaxLength(20)]
    [Required]
    public required string PilotId { get; set; }

    public PilotEntity Pilot { get; set; } = new();
}
