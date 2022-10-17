using Enum = NetTools.Enum;

namespace ntfy;

public class DelayUnit : Enum
{
    public static readonly DelayUnit Days = new(3, "d");
    public static readonly DelayUnit Hours = new(2, "h");
    public static readonly DelayUnit Minutes = new(1, "m");
    public static readonly DelayUnit Seconds = new(0, "s");

    internal readonly string Suffix;

    private DelayUnit(int id, string suffix) : base(id)
    {
        Suffix = suffix;
    }
}

public class DelayStatement : IDelay
{
    public string Value { get; }

    public DelayStatement(string value)
    {
        Value = value;
    }
}

public class DelayDuration : IDelay
{
    public string Value { get; }

    public DelayDuration(int value, DelayUnit unit)
    {
        Value = $"{value}{unit.Suffix}";
    }
}

public class DelayTime : IDelay
{
    public string Value { get; }

    public DelayTime(int timestamp)
    {
        Value = timestamp.ToString();
    }
}

public interface IDelay
{
    public string Value { get; }
}
