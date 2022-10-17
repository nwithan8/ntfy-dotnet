using Newtonsoft.Json;

namespace ntfy.Action;

/// <summary>
///     The base class for all actions that can be included on a <see cref="ReceivedMessage" /> or
///     <see cref="SendingMessage" />.
/// </summary>
public abstract class Action : IAction
{
    #region JSON Properties

    /// <summary>
    ///     The type of this action, as a string.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("action", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    private string ActionString => ActionType.ToString()!;

    /// <summary>
    ///     The label of this action.
    /// </summary>
    [JsonProperty("label", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public string Label { get; set; }

    #endregion

    /// <summary>
    ///     The type of this action, as a <see cref="ActionType" /> enum.
    /// </summary>
    [JsonIgnore]
    public abstract ActionType ActionType { get; }

    /// <summary>
    ///     Constructor for an action.
    /// </summary>
    /// <param name="label">The label for the action.</param>
    protected Action(string label)
    {
        Label = label;
    }
}

/// <summary>
///     The base interface for all actions that can be included on a <see cref="ReceivedMessage" /> or
///     <see cref="SendingMessage" />.
/// </summary>
public interface IAction
{
}
