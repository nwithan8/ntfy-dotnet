using Newtonsoft.Json;

namespace ntfy.Action;

public class View : Action
{
    #region JSON Properties

    [JsonProperty("clear", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Clear { get; set; } = false;

    [JsonProperty("url", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri Url { get; set; }

    #endregion

    [JsonIgnore]
    public override ActionType ActionType { get; } = ActionType.View;

    public View(string label, Uri url) : base(label)
    {
        Url = url;
    }
}
