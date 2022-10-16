using Newtonsoft.Json;

namespace ntfy.Action;

public class Broadcast : Action
{
    [JsonIgnore]
    public override ActionType ActionType { get; } = ntfy.ActionType.Broadcast;

    [JsonProperty("intent", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Intent { get; set; }

    [JsonProperty("extras", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? Extras { get; set; }

    [JsonProperty("clear", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Clear { get; set; } = false;
    
    public Broadcast(string label) : base(label)
    {
    }
}
