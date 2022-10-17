using Newtonsoft.Json;

namespace ntfy.Action;

public class Broadcast : Action
{
    #region JSON Properties

    [JsonProperty("clear", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Clear { get; set; } = false;

    [JsonProperty("extras", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? Extras { get; set; }

    [JsonProperty("intent", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Intent { get; set; }

    #endregion

    [JsonIgnore]
    public override ActionType ActionType { get; } = ActionType.Broadcast;

    public Broadcast(string label) : base(label)
    {
    }
}
