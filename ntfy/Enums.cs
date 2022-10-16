namespace ntfy;

public class EventType : NetTools.ValueEnum
{
    public static readonly EventType Open = new EventType(0, "open");
    public static readonly EventType KeepAlive = new EventType(1, "keepalive");
    public static readonly EventType Message = new EventType(2, "message");
    public static readonly EventType PollRequest = new EventType(3, "poll_request");

    private EventType(int id, object value) : base(id, value)
    {
    }
}

public class PriorityLevel : NetTools.ValueEnum
{
    public static readonly PriorityLevel Min = new(0, 1, "min");
    public static readonly PriorityLevel Low = new(1, 2, "low");
    public static readonly PriorityLevel Default = new(2, 3, "default");
    public static readonly PriorityLevel High = new(3, 4, "high");
    public static readonly PriorityLevel Max = new(4, 5, "max");

    internal string Word { get; }

    private PriorityLevel(int enumId, int id, string word) : base(enumId, id)
    {
        Word = word;
    }
}

public class ActionType : NetTools.ValueEnum
{
    public static readonly ActionType View = new(0, "view");
    public static readonly ActionType Broadcast = new(1, "broadcast");
    public static readonly ActionType Http = new(1, "http");

    private ActionType(int id, object value) : base(id, value)
    {
    }
}

internal class StreamType : NetTools.Enum
{
    public static readonly StreamType Json = new(0, "json");
    public static readonly StreamType WebSocket = new(1, "ws");

    internal string Endpoint { get; }

    private StreamType(int id, string endpoint) : base(id)
    {
        Endpoint = endpoint;
    }
}
