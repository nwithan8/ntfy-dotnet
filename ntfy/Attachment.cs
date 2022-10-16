using Newtonsoft.Json;

namespace ntfy;

public class Attachment
{
    [JsonProperty("name", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; } = null!;

    [JsonProperty("url", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public Uri Url { get; set; } = null!;

    [JsonProperty("type", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public string Type { get; set; } = null!;

    [JsonProperty("size", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public int Size { get; set; }

    [JsonProperty("expires", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    private int ExpiresTimestamp { get; set; }

    [JsonIgnore]
    public DateTime Expires
    {
        get => DateTimeOffset.FromUnixTimeSeconds(ExpiresTimestamp).DateTime;
        set => ExpiresTimestamp = (int)((DateTimeOffset)value).ToUnixTimeSeconds();
    }
}
