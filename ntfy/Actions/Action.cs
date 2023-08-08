using NetTools;
using Newtonsoft.Json;
using ntfy.Requests;
using ntfy.Responses;

namespace ntfy.Actions;

/// <summary>
///     The base class for all actions that can be included on a <see cref="ReceivedMessage" /> or
///     <see cref="SendingMessage" />.
/// </summary>
public abstract class Action : IAction
{
    /// <summary>
    ///     Constructor for an action.
    /// </summary>
    /// <param name="label">The label for the action.</param>
    protected Action(string label)
    {
        Label = label;
    }

    /// <summary>
    ///     The type of this action, as an <see cref="ActionType" /> enum.
    /// </summary>
    [JsonIgnore]
    public abstract ActionType Type { get; }

    internal static Action[] DataToActions(Dictionary<string, object>[]? data)
    {
        var actions = new List<Action>();

        if (data == null)
            return actions.ToArray();

        foreach (var entry in data)
        {
            if (!entry.ContainsKey("action"))
                continue;

            var entryString = JsonConvert.SerializeObject(entry);

            Action? action = null;

            var @switch = new SwitchCase
            {
                { ActionType.Http.ToString()!, () => action = JsonConvert.DeserializeObject<Http>(entryString) },
                {
                    ActionType.Broadcast.ToString()!,
                    () => action = JsonConvert.DeserializeObject<Broadcast>(entryString)
                },
                { ActionType.View.ToString()!, () => action = JsonConvert.DeserializeObject<View>(entryString) },
                { Scenario.Default, () => action = null }
            };

            @switch.MatchFirst((entry["action"] as string)!);

            if (action != null)
                actions.Add(action);
        }

        return actions.ToArray();
    }

    internal static Dictionary<string, object>[]? ActionsToData(Action[]? actions)
    {
        if (actions == null || actions.Length == 0)
            return null;

        var actionsDataString = JsonConvert.SerializeObject(actions);

        return JsonConvert.DeserializeObject<Dictionary<string, object>[]>(actionsDataString);
    }

    #region JSON Properties

    /// <summary>
    ///     The type of this action, as a string.
    ///     This representation is used by API calls, and is hidden from end users.
    /// </summary>
    [JsonProperty("action", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    private string ActionString => Type.ToString()!;

    /// <summary>
    ///     The label of this action.
    /// </summary>
    [JsonProperty("label", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public string Label { get; set; }

    #endregion
}

/// <summary>
///     The base interface for all actions that can be included on a <see cref="ReceivedMessage" /> or
///     <see cref="SendingMessage" />.
/// </summary>
public interface IAction
{
}