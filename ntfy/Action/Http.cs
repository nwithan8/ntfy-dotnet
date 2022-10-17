using Newtonsoft.Json;

namespace ntfy.Action;

public class Http : Action
{
    #region JSON Properties

    [JsonProperty("body", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Body { get; set; }

    [JsonProperty("clear", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Clear { get; set; } = false;

    [JsonProperty("headers", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string>? Headers { get; set; }

    [JsonProperty("method", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Method { get; set; }

    [JsonProperty("url", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri Url { get; set; }

    #endregion

    [JsonIgnore]
    public override ActionType ActionType { get; } = ActionType.Http;

    public Http(string label, Uri url) : base(label)
    {
        Url = url;
    }
}
