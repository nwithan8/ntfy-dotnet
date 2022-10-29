using Newtonsoft.Json;

namespace ntfy.Actions;

/// <summary>
///     A View action that can be included on a <see cref="ReceivedMessage" /> or <see cref="SendingMessage" />.
///     Opens a website or app when the action button is tapped.
/// </summary>
public class View : Action
{
    #region JSON Properties

    /// <summary>
    ///     Whether to clear the associated notification after this <see cref="View" /> action button is tapped.
    ///     Defaults to <c>false</c>.
    /// </summary>
    [JsonProperty("clear", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public bool? Clear { get; set; } = false;

    /// <summary>
    ///     The URL to open when this <see cref="View" /> action button is tapped.
    /// </summary>
    [JsonProperty("url", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
    public Uri Url { get; set; }

    #endregion

    /// <summary>
    ///     The type of this action, as an <see cref="ActionType" /> enum.
    /// </summary>
    [JsonIgnore]
    public override ActionType Type { get; } = ActionType.View;

    /// <summary>
    ///     Constructor for a View action.
    /// </summary>
    /// <param name="label">The label for the action.</param>
    /// <param name="url">The URL to open when the action is tapped.</param>
    public View(string label, Uri url) : base(label)
    {
        Url = url;
    }
}
