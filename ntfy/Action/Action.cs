using Newtonsoft.Json;

namespace ntfy.Action;

public abstract class Action
{
    [JsonProperty("action", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    private string ActionString => ActionType.ToString()!;

    [JsonIgnore]
    public abstract ActionType ActionType { get; }

    [JsonProperty("label", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public string Label { get; set; }

    protected Action(string label)
    {
        Label = label;
    }
}
