using Newtonsoft.Json;

namespace ntfy.Action;

/// <summary>
///     A Broadcast action that can be included on a <see cref="ReceivedMessage" /> or <see cref="SendingMessage" />.
///     Sends an Android broadcast intent when the action button is tapped.
///     Defaults to an intent that works specifically with the official ntfy Android app.
/// </summary>
public class Broadcast : Action
{
    #region JSON Properties

    /// <summary>
    ///     Whether to clear the associated notification after this <see cref="Broadcast" /> action button is tapped.
    ///     Defaults to <c>false</c>.
    /// </summary>
    [JsonProperty("clear", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Clear { get; set; } = false;

    /// <summary>
    ///     The dictionary representation of the Android intent extras of this <see cref="Broadcast" /> action.
    ///     Currently, only string extras are supported.
    /// </summary>
    [JsonProperty("extras", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? Extras { get; set; }

    /// <summary>
    ///     Android intent name of this <see cref="Broadcast" /> action.
    ///     Default is <c>io.heckel.ntfy.USER_ACTION</c>
    /// </summary>
    [JsonProperty("intent", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public string? Intent { get; set; } = "io.heckel.ntfy.USER_ACTION";

    #endregion

    /// <summary>
    ///     The type of this action, as a <see cref="ActionType" /> enum.
    /// </summary>
    [JsonIgnore]
    public override ActionType ActionType { get; } = ActionType.Broadcast;

    /// <summary>
    ///     Constructor for a Broadcast action.
    /// </summary>
    /// <param name="label">The label for the action.</param>
    public Broadcast(string label) : base(label)
    {
    }
}
