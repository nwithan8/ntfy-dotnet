using NetTools;
using Newtonsoft.Json;

namespace ntfy;

/// <summary>
///     A message received from the server.
/// </summary>
public class ReceivedMessage
{
    #region JSON Properties

    /// <summary>
    ///     A list of actions included in this message.
    ///     Since actions can be of different types, this is a list of dictionary representations of the actions.
    /// </summary>
    [JsonProperty("actions")]
    public Dictionary<string, object>[]? Actions { get; set; }

    /// <summary>
    ///     The attachment included in this message.
    /// </summary>
    [JsonProperty("attachment")]
    public Attachment? Attachment { get; set; }

    /// <summary>
    ///     The URL opened when the associated notification for this message is clicked.
    /// </summary>
    [JsonProperty("click")]
    public Uri? Click { get; set; }

    /// <summary>
    ///     Identifier for this message.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    ///     The body of this message.
    ///     Always present in <c>EventType.Message</c> events.
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    ///     A list of tags of this message that may or not map to emojis.
    /// </summary>
    [JsonProperty("tags")]
    public string[]? Tags { get; set; }

    /// <summary>
    ///     The title of this message.
    ///     Defaults to <c>ntfy.sh/{topic}</c>
    /// </summary>
    [JsonProperty("title")]
    public string? Title { get; set; }

    /// <summary>
    ///     The event type of this message.
    ///     Typically you'd be only interested in message.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("event")]
    private string EventString { get; set; } = string.Empty;

    /// <summary>
    ///     The priority of this message.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("priority")]
    private int? PriorityInt { get; set; }

    /// <summary>
    ///     The datetime of this message, as a Unix time stamp.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("time")]
    private int Timestamp { get; set; }

    /// <summary>
    ///     A comma-separated list of topics the message is associated with.
    ///     Only one for all message events, but may be a list in open events.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("topic")]
    private string TopicString { get; set; } = string.Empty;

    #endregion

    /// <summary>
    ///     The event type of this message.
    ///     Typically you'd be only interested in <c>EventType.Message</c>.
    /// </summary>
    [JsonIgnore]
    public EventType? Event
    {
        get => ValueEnum.FromValue<EventType>(EventString);
        set => EventString = (string)(value?.Value ?? "");
    }

    /// <summary>
    ///     The priority of this message.
    /// </summary>
    [JsonIgnore]
    public PriorityLevel? Priority
    {
        get => ValueEnum.FromValue<PriorityLevel>(PriorityInt);
        set => PriorityInt = (int)(value?.Value ?? 0);
    }

    /// <summary>
    ///     The datetime of this message, as a DateTime object.
    /// </summary>
    [JsonIgnore]
    public DateTime Time
    {
        get => DateTimeOffset.FromUnixTimeSeconds(Timestamp).DateTime;
        set => Timestamp = (int)((DateTimeOffset)value).ToUnixTimeSeconds();
    }

    /// <summary>
    ///     A list of topics the message is associated with.
    ///     Only one for all message events, but may be a list in open events.
    /// </summary>
    [JsonIgnore]
    public List<string> Topic
    {
        get => TopicString.Split(',').ToList();
        set => TopicString = string.Join(",", value);
    }
}
