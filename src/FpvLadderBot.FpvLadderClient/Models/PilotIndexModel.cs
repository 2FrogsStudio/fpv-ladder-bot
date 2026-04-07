using YamlDotNet.Serialization;

namespace FpvLadderBot.Models;

public class PilotIndexModel {
    [YamlMember(Alias = "pilot_id")]
    public string PilotId { get; set; }
    
    [YamlMember(Alias = "class")]
    public string Class { get; set; }
    
    [YamlMember(Alias = "pilot_name")]
    public string PilotName { get; set; }
    
    [YamlMember(Alias = "rating_value")]
    public uint RatingValue { get; set; }
    
    [YamlMember(Alias = "rating_num")]
    public uint RatingNum { get; set; }
    
    [YamlMember(Alias = "event_date")]
    public DateTimeOffset EventDate { get; set; }
}
