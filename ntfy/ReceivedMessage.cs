using NetTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ntfy;

public class ReceivedMessage
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!;

    [JsonProperty("time")]
    private int Timestamp { get; set; }

    [JsonIgnore]
    public DateTime Time
    {
        get => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;
        set => Timestamp = (int)((DateTimeOffset)value).ToUnixTimeSeconds();
    }

    [JsonProperty("event")]
    private string EventString { get; set; } = null!;

    [JsonIgnore]
    public EventType? Event
    {
        get => ValueEnum.FromValue<EventType>(EventString);
        set => EventString = (string)(value?.Value ?? "");
    }

    [JsonProperty("topic")]
    public string Topic { get; set; } = null!;

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("tags")]
    public string[]? Tags { get; set; }

    [JsonProperty("priority")]
    private int? PriorityInt { get; set; }

    [JsonIgnore]
    public PriorityLevel? Priority
    {
        get => ValueEnum.FromValue<PriorityLevel>(PriorityInt);
        set => PriorityInt = (int)(value?.Value ?? 0);
    }

    [JsonProperty("click")]
    public Uri? Click { get; set; }

    [JsonProperty("actions")]
    public Dictionary<string, object>[]? Actions { get; set; }

    [JsonProperty("attachment")]
    public Attachment? Attachment { get; set; }
}
