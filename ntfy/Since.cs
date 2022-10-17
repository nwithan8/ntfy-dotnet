namespace ntfy;

public class Since
{
    public static readonly Since All = new("all");
    internal readonly string Value;

    public Since(DelayDuration time)
    {
        Value = time.Value;
    }

    public Since(DelayTime time)
    {
        Value = time.Value;
    }

    public Since(string messageId)
    {
        Value = messageId;
    }

    public static implicit operator Since(int timestamp)
    {
        return new Since(new DelayTime(timestamp));
    }

    public static implicit operator Since(DelayDuration time)
    {
        return new Since(time);
    }

    public static implicit operator Since(DelayTime time)
    {
        return new Since(time);
    }

    public static implicit operator Since(string messageId)
    {
        return new Since(messageId);
    }
}
