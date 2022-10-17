using NetTools;

namespace ntfy;

/// <summary>
///     An enum representation of an event type.
/// </summary>
public class EventType : ValueEnum
{
    /// <summary>
    ///     Enum representation of a keepalive event type.
    /// </summary>
    public static readonly EventType KeepAlive = new(1, "keepalive");
    /// <summary>
    ///     Enum representation of a message event type.
    /// </summary>
    public static readonly EventType Message = new(2, "message");
    /// <summary>
    ///     Enum representation of an open event type.
    /// </summary>
    public static readonly EventType Open = new(0, "open");
    /// <summary>
    ///     Enum representation of a poll_request event type.
    /// </summary>
    public static readonly EventType PollRequest = new(3, "poll_request");

    /// <summary>
    ///     Constructor for the EventType enum.
    /// </summary>
    /// <param name="id">Internal enum ID.</param>
    /// <param name="value">String representation of the event type.</param>
    private EventType(int id, string value) : base(id, value)
    {
    }
}

/// <summary>
///     An enum representation of a priority level.
/// </summary>
public class PriorityLevel : ValueEnum
{
    /// <summary>
    ///     Enum representation of a default priority level.
    /// </summary>
    public static readonly PriorityLevel Default = new(2, 3, "default");
    /// <summary>
    ///     Enum representation of a high priority level.
    /// </summary>
    public static readonly PriorityLevel High = new(3, 4, "high");
    /// <summary>
    ///     Enum representation of a low priority level.
    /// </summary>
    public static readonly PriorityLevel Low = new(1, 2, "low");
    /// <summary>
    ///     Enum representation of a max priority level.
    /// </summary>
    public static readonly PriorityLevel Max = new(4, 5, "max");
    /// <summary>
    ///     Enum representation of a min priority level.
    /// </summary>
    public static readonly PriorityLevel Min = new(0, 1, "min");

    /// <summary>
    ///     Word representation of the priority level.
    /// </summary>
    internal string Word { get; }

    /// <summary>
    ///     Constructor for the PriorityLevel enum.
    /// </summary>
    /// <param name="enumId">Internal enum ID.</param>
    /// <param name="id">Priority level integer.</param>
    /// <param name="word">Priority level word.</param>
    private PriorityLevel(int enumId, int id, string word) : base(enumId, id)
    {
        Word = word;
    }
}

/// <summary>
///     An enum representation of a action type.
/// </summary>
public class ActionType : ValueEnum
{
    /// <summary>
    ///     Enum representation of a broadcast action type.
    /// </summary>
    public static readonly ActionType Broadcast = new(1, "broadcast");
    /// <summary>
    ///     Enum representation of an http action type.
    /// </summary>
    public static readonly ActionType Http = new(1, "http");
    /// <summary>
    ///     Enum representation of a view action type.
    /// </summary>
    public static readonly ActionType View = new(0, "view");

    /// <summary>
    ///     Constructor for the ActionType enum.
    /// </summary>
    /// <param name="id">Internal enum ID.</param>
    /// <param name="value">String representation of the action type.</param>
    private ActionType(int id, object value) : base(id, value)
    {
    }
}

/// <summary>
///     An enum representation of a stream type.
/// </summary>
internal class StreamType : ValueEnum
{
    /// <summary>
    ///     Enum representation of a JSON stream type.
    /// </summary>
    public static readonly StreamType Json = new(0, "json");
    /// <summary>
    ///     Enum representation of a websocket stream type.
    /// </summary>
    public static readonly StreamType WebSocket = new(1, "ws");

    /// <summary>
    ///     Constructor for the StreamType enum.
    /// </summary>
    /// <param name="id">Internal enum ID.</param>
    /// <param name="endpoint">Endpoint string for the stream type.</param>
    private StreamType(int id, string endpoint) : base(id, endpoint)
    {
    }
}
