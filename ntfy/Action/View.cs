using Newtonsoft.Json;

namespace ntfy.Action;

public class View : Action
{
    [JsonIgnore]
    public override ActionType ActionType { get; } = ntfy.ActionType.View;

    [JsonProperty("url", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri Url { get; set; }

    [JsonProperty("clear", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Clear { get; set; } = false;

    public View(string label, Uri url) : base(label)
    {
        Url = url;
    }
}
