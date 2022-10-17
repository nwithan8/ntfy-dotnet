using Newtonsoft.Json;

namespace ntfy.Action;

public abstract class Action
{
    #region JSON Properties

    [JsonProperty("action", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    private string ActionString => ActionType.ToString()!;

    [JsonProperty("label", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public string Label { get; set; }

    #endregion

    [JsonIgnore]
    public abstract ActionType ActionType { get; }

    protected Action(string label)
    {
        Label = label;
    }
}
