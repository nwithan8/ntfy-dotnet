using NetTools;
using NetTools.HTTP;
using Newtonsoft.Json;

namespace ntfy;

public class SendingMessage
{
    #region JSON Properties

    [JsonProperty("actions", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Action.Action[]? Actions { get; set; }

    [JsonProperty("attach", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri[]? Attach { get; set; }

    [JsonProperty("click", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri? Click { get; set; }

    [JsonProperty("delay", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? DelayString { get; set; }

    [JsonProperty("email", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Email { get; set; }

    [JsonProperty("filename", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Filename { get; set; }

    [JsonProperty("icon", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Icon { get; set; }

    [JsonProperty("message", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }

    [JsonProperty("tags", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string[]? Tags { get; set; }

    [JsonProperty("title", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Title { get; set; }
    [JsonProperty("topic", Required = Required.Always)]
    internal string Topic { get; set; } = null!;

    [JsonProperty("cache", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private string CacheString { get; set; } = "yes";

    [JsonProperty("firebase", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private string FirebaseString { get; set; } = "yes";

    [JsonProperty("priority", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private int? PriorityInt { get; set; }

    [JsonProperty("unifiedpush", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    private int UnifiedPushInt { get; set; }

    #endregion

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
                { Scenario.Default, () => throw new Exception("Invalid cache parameter.") }
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
                { Scenario.Default, () => throw new Exception("Invalid cache parameter.") }
            };
            @switch.MatchFirst(value);
        }
    }

    [JsonIgnore]
    public IDelay? Delay
    {
        // no getter since it could be one of multiple different constructors
        set => DelayString = value?.Value;
    }

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
                { Scenario.Default, () => throw new Exception("Invalid firebase parameter.") }
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
                { Scenario.Default, () => throw new Exception("Invalid firebase parameter.") }
            };
            @switch.MatchFirst(value);
        }
    }

    [JsonIgnore]
    public PriorityLevel? Priority
    {
        get => ValueEnum.FromValue<PriorityLevel>(PriorityInt);
        set => PriorityInt = (int)(value?.Value ?? 0);
    }

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
                { Scenario.Default, () => throw new Exception("Invalid unified push parameter.") }
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
                { Scenario.Default, () => throw new Exception("Invalid unified push parameter.") }
            };
            @switch.MatchFirst(value);
        }
    }

    internal string ToData(string topic)
    {
        Topic = topic;
        return JsonSerialization.ConvertObjectToJson(this);
    }
}
