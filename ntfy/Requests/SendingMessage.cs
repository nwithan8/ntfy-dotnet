using NetTools;
using NetTools.HTTP;
using Newtonsoft.Json;
using ntfy.Actions;
using ntfy.Filters;
using Action = ntfy.Actions.Action;

namespace ntfy.Requests;

/// <summary>
///     A message sent to the server.
/// </summary>
public class SendingMessage
{
    #region JSON Properties

    /// <summary>
    ///     A list of actions to include in this message.
    ///     Available actions include <see cref="View" />, <see cref="Broadcast" /> and
    ///     <see cref="ntfy.Actions.Http" />.
    /// </summary>
    [JsonProperty("actions", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Action[]? Actions { get; set; }

    /// <summary>
    ///     A URL to send as an attachment in this message.
    ///     This is an alternative to sending the file itself to the server via <see cref="Filename" />.
    /// </summary>
    [JsonProperty("attach", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri[]? Attach { get; set; }

    /// <summary>
    ///     The URL to open when the associated notification for this message is clicked.
    /// </summary>
    [JsonProperty("click", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri? Click { get; set; }

    [JsonProperty("email", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Email { get; set; }

    /// <summary>
    ///     The filename of the file to include in this message.
    ///     This file will be sent to the server. If you want to send a URL instead, use <see cref="Attach" /> instead.
    /// </summary>
    [JsonProperty("filename", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Filename { get; set; }

    /// <summary>
    ///     The URL to send as the associated notification's icon.
    /// </summary>
    [JsonProperty("icon", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Icon { get; set; }

    /// <summary>
    ///     The main body of this message to show in the associated notification.
    /// </summary>
    [JsonProperty("message", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }

    /// <summary>
    ///     Tags to include in the message.
    /// </summary>
    [JsonProperty("tags", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string[]? Tags { get; set; }

    /// <summary>
    ///     The title of this message.
    /// </summary>
    [JsonProperty("title", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Title { get; set; }

    /// <summary>
    ///     The topic to send this message to.
    ///     This parameter is set internally during the sending process, and should not be set manually by the user.
    /// </summary>
    [JsonProperty("topic", Required = Required.Always)]
    internal string Topic { get; set; } = null!;

    /// <summary>
    ///     Whether to enable or disable caching for this message, as a string.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("cache", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private string CacheString { get; set; } = "yes";

    /// <summary>
    ///     The string representation of the delay to wait before sending this message.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("delay", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private string? DelayString { get; set; }

    /// <summary>
    ///     Whether to enable or disable sending this message to Firebase, as a string.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("firebase", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private string FirebaseString { get; set; } = "yes";

    /// <summary>
    ///     The priority level of this message, as an integer.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("priority", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private int? PriorityInt { get; set; }

    /// <summary>
    ///     Whether to enable or disable UnifiedPush for this message, as a string.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("unifiedpush", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private int UnifiedPushInt { get; set; }

    #endregion

    /// <summary>
    ///     Whether to enable or disable caching for this message.
    /// </summary>
    /// <exception cref="InvalidParameterException">An invalid option was parsed.</exception>
    [JsonIgnore]
    public bool Cache
    {
        get
        {
            var value = false;
            var @switch = new SwitchCase
            {
                { "yes", () => value = true },
                { "no", () => value = false },
                { Scenario.Default, () => throw new InvalidParameterException("cache") }
            };
            @switch.MatchFirst(CacheString);
            return value;
        }
        set
        {
            var @switch = new SwitchCase
            {
                { true, () => CacheString = "yes" },
                { false, () => CacheString = "no" },
                { Scenario.Default, () => throw new InvalidParameterException("cache") }
            };
            @switch.MatchFirst(value);
        }
    }

    /// <summary>
    ///     The delay to wait before sending the message.
    /// </summary>
    [JsonIgnore]
    public Delay? Delay
    {
        // no getter since it could be one of multiple different constructors
        set => DelayString = value?.Value;
    }

    /// <summary>
    ///     Whether to enable or disable sending this message to Firebase.
    /// </summary>
    /// <exception cref="InvalidParameterException">An invalid option was parsed.</exception>
    [JsonIgnore]
    public bool Firebase
    {
        get
        {
            var value = false;
            var @switch = new SwitchCase
            {
                { "yes", () => value = true },
                { "no", () => value = false },
                { Scenario.Default, () => throw new InvalidParameterException("firebase") }
            };
            @switch.MatchFirst(FirebaseString);
            return value;
        }
        set
        {
            var @switch = new SwitchCase
            {
                { true, () => FirebaseString = "yes" },
                { false, () => FirebaseString = "no" },
                { Scenario.Default, () => throw new InvalidParameterException("firebase") }
            };
            @switch.MatchFirst(value);
        }
    }

    /// <summary>
    ///     The priority level of this message.
    /// </summary>
    [JsonIgnore]
    public PriorityLevel? Priority
    {
        get => ValueEnum.FromValue<PriorityLevel>(PriorityInt);
        set => PriorityInt = (int)(value?.Value ?? 0);
    }

    /// <summary>
    ///     Whether to enable or disable UnifiedPush for this message.
    /// </summary>
    /// <exception cref="InvalidParameterException">An invalid option was parsed.</exception>
    [JsonIgnore]
    public bool UnifiedPush
    {
        get
        {
            var value = false;
            var @switch = new SwitchCase
            {
                { 1, () => value = true },
                { 0, () => value = false },
                { Scenario.Default, () => throw new InvalidParameterException("unifiedpush") }
            };
            @switch.MatchFirst(UnifiedPushInt);
            return value;
        }
        set
        {
            var @switch = new SwitchCase
            {
                { true, () => UnifiedPushInt = 1 },
                { false, () => UnifiedPushInt = 0 },
                { Scenario.Default, () => throw new InvalidParameterException("unifiedpush") }
            };
            @switch.MatchFirst(value);
        }
    }

    /// <summary>
    ///     Convert this message to a JSON data string.
    /// </summary>
    /// <param name="topic">Topic this message will be sent to.</param>
    /// <returns>A JSON data string representation of this message.</returns>
    internal string ToData(string topic)
    {
        Topic = topic;
        return JsonSerialization.ConvertObjectToJson(this);
    }
}
