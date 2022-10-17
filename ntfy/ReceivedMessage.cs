using NetTools;
using Newtonsoft.Json;

namespace ntfy;

public class ReceivedMessage
{
    #region JSON Properties

    [JsonProperty("actions")]
    public Dictionary<string, object>[]? Actions { get; set; }

    [JsonProperty("attachment")]
    public Attachment? Attachment { get; set; }

    [JsonProperty("click")]
    public Uri? Click { get; set; }
    [JsonProperty("id")]
    public string Id { get; set; } = null!;

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("tags")]
    public string[]? Tags { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("topic")]
    public string Topic { get; set; } = null!;

    [JsonProperty("event")]
    private string EventString { get; set; } = null!;

    [JsonProperty("priority")]
    private int? PriorityInt { get; set; }

    [JsonProperty("time")]
    private int Timestamp { get; set; }

    #endregion

    [JsonIgnore]
    public EventType? Event
    {
        get => ValueEnum.FromValue<EventType>(EventString);
        set => EventString = (string)(value?.Value ?? "");
    }

    [JsonIgnore]
    public PriorityLevel? Priority
    {
        get => ValueEnum.FromValue<PriorityLevel>(PriorityInt);
        set => PriorityInt = (int)(value?.Value ?? 0);
    }

    [JsonIgnore]
    public DateTime Time
    {
        get => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;
        set => Timestamp = (int)((DateTimeOffset)value).ToUnixTimeSeconds();
    }
}
